using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using SkillNet.Application.Interfaces;

namespace SkillNet.Infrastructure.Services;

public class EmailService(IConfiguration configuration) : IEmailService
{
    public async Task SendAsync(string to, string subject, string htmlBody, CancellationToken ct = default)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(
            configuration["Email:FromName"] ?? "Skill.Net",
            configuration["Email:FromAddress"] ?? "noreply@skillnet.vn"));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;

        var builder = new BodyBuilder { HtmlBody = htmlBody };
        message.Body = builder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(
            configuration["Email:Host"],
            int.Parse(configuration["Email:Port"] ?? "587"),
            SecureSocketOptions.StartTls, ct);
        await client.AuthenticateAsync(
            configuration["Email:Username"],
            configuration["Email:Password"], ct);
        await client.SendAsync(message, ct);
        await client.DisconnectAsync(true, ct);
    }
}
