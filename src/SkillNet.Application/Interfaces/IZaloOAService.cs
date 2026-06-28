namespace SkillNet.Application.Interfaces;

public interface IZaloOAService
{
    Task SendZnsAsync(string phone, string templateId, Dictionary<string, string> templateData, CancellationToken ct = default);
}
