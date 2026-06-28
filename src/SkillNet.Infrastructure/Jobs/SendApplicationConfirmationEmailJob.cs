using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SkillNet.Application.Features.Membership.Jobs;
using SkillNet.Application.Interfaces;
using SkillNet.Infrastructure.Helpers;
using SkillNet.Infrastructure.Persistence;
using System.Diagnostics;

namespace SkillNet.Infrastructure.Jobs;

[AutomaticRetry(Attempts = 3, DelaysInSeconds = [60, 300, 900])]
public class SendApplicationConfirmationEmailJob(
    IEmailService emailService,
    SkillNetDbContext db,
    ILogger<SendApplicationConfirmationEmailJob> logger) : IConfirmationEmailJob
{
    public async Task ExecuteAsync(Guid applicationId, string userEmail, string userName, string packageName)
    {
        logger.LogInformation("Sending confirmation email. ApplicationId={ApplicationId} To={Email}",
            applicationId, LogMaskHelper.MaskEmail(userEmail));

        var sw = Stopwatch.StartNew();

        var subject = "Don dang ky hoi vien da duoc gui thanh cong";
        var body = $"""
            <h2>Xin chao {userName},</h2>
            <p>Chung toi da nhan duoc don dang ky goi <strong>{packageName}</strong> cua ban.</p>
            <p>Ma don: <strong>{applicationId}</strong></p>
            <p>Ban quan ly se xet duyet don trong 1-2 ngay lam viec. Chung toi se thong bao ket qua qua email va Zalo.</p>
            <p>Tran trong,<br/>Skill.Net</p>
            """;

        await emailService.SendAsync(userEmail, subject, body);

        sw.Stop();
        logger.LogInformation("Confirmation email sent successfully. ApplicationId={ApplicationId} DurationMs={Ms}",
            applicationId, sw.ElapsedMilliseconds);
    }
}
