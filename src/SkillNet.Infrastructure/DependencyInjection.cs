using Hangfire;
using Hangfire.SqlServer;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SkillNet.Application.Features.Membership.Jobs;
using SkillNet.Application.Interfaces;
using SkillNet.Domain.Interfaces;
using SkillNet.Infrastructure.Identity;
using SkillNet.Infrastructure.Jobs;
using SkillNet.Infrastructure.Persistence;
using SkillNet.Infrastructure.Persistence.Repositories;
using SkillNet.Infrastructure.Services;

namespace SkillNet.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<SkillNetDbContext>(opts =>
            opts.UseSqlServer(configuration.GetConnectionString("Default")));

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        services.AddIdentity<ApplicationUser, IdentityRole>(opts =>
        {
            opts.Password.RequireDigit = true;
            opts.Password.RequiredLength = 8;
            opts.SignIn.RequireConfirmedEmail = false;
        })
        .AddEntityFrameworkStores<SkillNetDbContext>()
        .AddDefaultTokenProviders();

        services.ConfigureApplicationCookie(opts =>
        {
            opts.LoginPath = "/Account/Login";
            opts.LogoutPath = "/Account/Logout";
            opts.AccessDeniedPath = "/Account/AccessDenied";
        });

        services.AddScoped<IMembershipApplicationRepository, MembershipApplicationRepository>();
        services.AddScoped<IServicePackageRepository, ServicePackageRepository>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IZaloOAService, ZaloOAService>();
        services.AddScoped<IFileStorageService, LocalFileStorageService>();
        services.AddScoped<IConfirmationEmailJob, SendApplicationConfirmationEmailJob>();
        services.AddScoped<IResultNotificationJob, SendApplicationResultNotificationJob>();
        services.AddScoped<RemindAdminPendingApplicationsJob>();

        services.AddHangfire(cfg => cfg
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(configuration.GetConnectionString("Default"),
                new SqlServerStorageOptions { CommandBatchMaxTimeout = TimeSpan.FromMinutes(5) }));
        services.AddHangfireServer();

        services.AddHttpClient("ZaloOA", client =>
        {
            client.BaseAddress = new Uri(configuration["Zalo:ApiUrl"] ?? "https://openapi.zalo.me");
            client.DefaultRequestHeaders.Add("access_token", configuration["Zalo:AccessToken"]);
        });

        return services;
    }
}
