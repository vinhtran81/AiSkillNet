using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SkillNet.Application.Interfaces;
using SkillNet.Domain.Enums;
using SkillNet.Infrastructure.Helpers;
using SkillNet.Infrastructure.Persistence;

namespace SkillNet.Infrastructure.Jobs;

public class RemindAdminPendingApplicationsJob(
    SkillNetDbContext db,
    IEmailService emailService,
    IConfiguration configuration,
    ILogger<RemindAdminPendingApplicationsJob> logger)
{
    public async Task ExecuteAsync()
    {
        var overduePending = await db.MembershipApplications
            .AsNoTracking()
            .Where(a => a.Status == ApplicationStatus.Pending
                        && a.CreatedAt <= DateTime.UtcNow.AddDays(-3))
            .CountAsync();

        if (overduePending == 0)
        {
            logger.LogInformation("No overdue pending applications. Skipping admin reminder.");
            return;
        }

        var adminEmail = configuration["AdminEmail"] ?? "admin@skillnet.vn";
        logger.LogInformation("Admin reminder: {Count} applications pending > 3 days. Sending alert to {AdminEmail}",
            overduePending, LogMaskHelper.MaskEmail(adminEmail));

        var body = $"<h2>Nhac nho Admin</h2><p>Hien co <strong>{overduePending}</strong> don dang ky dang cho xet duyet qua 3 ngay.</p>";
        await emailService.SendAsync(adminEmail, $"[Skill.Net] {overduePending} don dang ky can xet duyet", body);
    }
}
