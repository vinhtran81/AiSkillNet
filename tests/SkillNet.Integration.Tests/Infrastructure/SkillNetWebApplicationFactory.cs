using Hangfire;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SkillNet.Application.Interfaces;
using SkillNet.Domain.Entities;
using SkillNet.Infrastructure.Persistence;

namespace SkillNet.Integration.Tests.Infrastructure;

public class SkillNetWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private const string TestConnectionString =
        "Server=VTRAN\\SQL2025;Database=SkillNetIntegrationTest;User Id=sa;Password=123456;TrustServerCertificate=True;MultipleActiveResultSets=true";

    public Guid SeedPackageId { get; private set; }
    private IServiceScope _scope = null!;
    private SkillNetDbContext _db = null!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureTestServices(services =>
        {
            // Replace DbContext with test database
            services.RemoveAll<DbContextOptions<SkillNetDbContext>>();
            services.AddDbContext<SkillNetDbContext>(opts =>
                opts.UseSqlServer(TestConnectionString));

            // Replace Hangfire with no-op background job client
            services.RemoveAll<IBackgroundJobClient>();
            services.AddSingleton<IBackgroundJobClient>(new Moq.Mock<IBackgroundJobClient>().Object);

            // Mock external services
            services.RemoveAll<IEmailService>();
            services.RemoveAll<IZaloOAService>();
            services.AddTransient<IEmailService, FakeEmailService>();
            services.AddTransient<IZaloOAService, FakeZaloService>();

            // Override authentication with test scheme
            services.AddAuthentication(opts =>
            {
                opts.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                opts.DefaultChallengeScheme = TestAuthHandler.SchemeName;
                opts.DefaultForbidScheme = TestAuthHandler.SchemeName;
            })
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });
        });
    }

    public async Task InitializeAsync()
    {
        _scope = Services.CreateScope();
        _db = _scope.ServiceProvider.GetRequiredService<SkillNetDbContext>();
        await _db.Database.EnsureCreatedAsync();

        // Seed roles
        var roleManager = _scope.ServiceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.RoleManager<Microsoft.AspNetCore.Identity.IdentityRole>>();
        foreach (var role in new[] { "Admin", "Member" })
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new Microsoft.AspNetCore.Identity.IdentityRole(role));

        // Seed a service package
        var pkg = new ServicePackage
        {
            Id = Guid.Parse("11111111-0000-0000-0000-000000000001"),
            Name = "Tiêu chuẩn Test",
            Description = "Gói test",
            Price = 500_000m,
            DurationMonths = 12,
            Benefits = "[\"Quyền lợi test\"]",
            IsActive = true,
            DisplayOrder = 1,
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        if (!await _db.ServicePackages.AnyAsync())
        {
            _db.ServicePackages.Add(pkg);
            await _db.SaveChangesAsync();
        }

        SeedPackageId = pkg.Id;
    }

    public HttpClient CreateAuthenticatedClient(string role = "Member", string? userId = null)
    {
        var client = CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        var id = userId ?? $"test-user-{role.ToLower()}";
        client.DefaultRequestHeaders.Add("X-Test-UserId", id);
        client.DefaultRequestHeaders.Add("X-Test-Role", role);
        return client;
    }

    public HttpClient CreateAnonClient()
        => CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

    public async Task<Guid> SeedPendingApplicationAsync(string userId)
    {
        var app = MembershipApplication.Create(
            userId, SeedPackageId,
            "Test User", new DateOnly(1990, 1, 1),
            "0901234567", "Test Address", null, null);

        _db.MembershipApplications.Add(app);
        await _db.SaveChangesAsync();
        return app.Id;
    }

    public async Task<MembershipApplication?> GetApplicationAsync(Guid id)
        => await _db.MembershipApplications
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(a => a.Id == id);

    public new Task DisposeAsync()
    {
        _scope?.Dispose();
        return Task.CompletedTask;
    }
}
