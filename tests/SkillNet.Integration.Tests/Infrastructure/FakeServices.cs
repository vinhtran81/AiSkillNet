using SkillNet.Application.Interfaces;

namespace SkillNet.Integration.Tests.Infrastructure;

public class FakeEmailService : IEmailService
{
    public Task SendAsync(string to, string subject, string htmlBody, CancellationToken ct = default)
        => Task.CompletedTask;
}

public class FakeZaloService : IZaloOAService
{
    public Task SendZnsAsync(string phone, string templateId, Dictionary<string, string> templateData, CancellationToken ct = default)
        => Task.CompletedTask;
}
