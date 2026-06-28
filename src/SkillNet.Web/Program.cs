using Hangfire;
using Microsoft.AspNetCore.Identity;
using Serilog;
using SkillNet.Application;
using SkillNet.Infrastructure;
using SkillNet.Infrastructure.Identity;
using SkillNet.Infrastructure.Jobs;
using SkillNet.Infrastructure.Persistence;
using SkillNet.Infrastructure.Persistence.Seeders;
using SkillNet.Web.Infrastructure;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("Hangfire", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "SkillNet")
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddOutputCache();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHealthChecks()
    .AddSqlServer(
        builder.Configuration.GetConnectionString("Default")!,
        name: "sql-server",
        tags: ["db", "ready"])
    .AddHangfire(
        opts => opts.MinimumAvailableServers = 1,
        name: "hangfire",
        tags: ["jobs", "ready"]);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSerilogRequestLogging();
app.UseRouting();
app.UseOutputCache();
app.UseAuthentication();
app.UseAuthorization();

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = [new HangfireAuthorizationFilter()]
});

app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = hc => hc.Tags.Contains("ready")
});
app.MapHealthChecks("/health");

app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SkillNetDbContext>();
    await db.Database.EnsureCreatedAsync();

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    foreach (var role in new[] { "Admin", "Member" })
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    await ServicePackageSeeder.SeedAsync(db);

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    await UserSeeder.SeedAsync(userManager);
}

RecurringJob.AddOrUpdate<RemindAdminPendingApplicationsJob>(
    "remind-admin-pending",
    job => job.ExecuteAsync(),
    "0 1 * * *");

app.Run();

public partial class Program { }
