using Hangfire;
using Microsoft.Extensions.Logging;
using SkillNet.Application.Features.Membership.Jobs;
using SkillNet.Application.Interfaces;
using SkillNet.Domain.Enums;
using SkillNet.Infrastructure.Helpers;

namespace SkillNet.Infrastructure.Jobs;

[AutomaticRetry(Attempts = 3, DelaysInSeconds = [60, 300, 900])]
public class SendApplicationResultNotificationJob(
    IEmailService emailService,
    IZaloOAService zaloService,
    ILogger<SendApplicationResultNotificationJob> logger) : IResultNotificationJob
{
    public async Task ExecuteAsync(Guid applicationId, ApplicationStatus status,
        string userEmail, string? phoneNumber, string? membershipCode, string? rejectionReason)
    {
        logger.LogInformation("Sending result notification. ApplicationId={ApplicationId} Status={Status}",
            applicationId, status);

        if (!string.IsNullOrEmpty(userEmail))
        {
            var (subject, body) = BuildEmail(status, membershipCode, rejectionReason);
            await emailService.SendAsync(userEmail, subject, body);
        }

        if (!string.IsNullOrEmpty(phoneNumber))
        {
            await SendZaloNotification(applicationId, status, phoneNumber, membershipCode, rejectionReason);
        }
        else
        {
            logger.LogWarning("Zalo OA notification skipped — no phone number on record. ApplicationId={ApplicationId}",
                applicationId);
        }
    }

    private (string subject, string body) BuildEmail(ApplicationStatus status, string? membershipCode, string? rejectionReason)
    {
        return status switch
        {
            ApplicationStatus.Approved => (
                "Chuc mung! Don dang ky hoi vien da duoc phe duyet",
                $"<h2>Chuc mung!</h2><p>Don dang ky hoi vien cua ban da duoc <strong>phe duyet</strong>.</p><p>Ma hoi vien: <strong>{membershipCode}</strong></p>"),
            ApplicationStatus.Rejected => (
                "Thong bao ket qua don dang ky hoi vien",
                $"<h2>Thong bao</h2><p>Rat tiec, don dang ky cua ban khong duoc phe duyet.</p><p>Ly do: {rejectionReason}</p><p>Ban co the nop don moi sau khi bo sung thong tin.</p>"),
            _ => ("Cap nhat trang thai don", "<p>Don dang ky cua ban da duoc cap nhat.</p>")
        };
    }

    private async Task SendZaloNotification(Guid applicationId, ApplicationStatus status,
        string phone, string? membershipCode, string? rejectionReason)
    {
        var templateId = status == ApplicationStatus.Approved ? "approved_zns_template" : "rejected_zns_template";
        var data = new Dictionary<string, string>
        {
            ["membership_code"] = membershipCode ?? string.Empty,
            ["rejection_reason"] = rejectionReason ?? string.Empty
        };
        await zaloService.SendZnsAsync(phone, templateId, data);
    }
}
