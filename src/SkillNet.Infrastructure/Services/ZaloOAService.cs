using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SkillNet.Application.Interfaces;
using System.Net.Http.Json;

namespace SkillNet.Infrastructure.Services;

public class ZaloOAService(
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration,
    ILogger<ZaloOAService> logger) : IZaloOAService
{
    public async Task SendZnsAsync(string phone, string templateId, Dictionary<string, string> templateData, CancellationToken ct = default)
    {
        var client = httpClientFactory.CreateClient("ZaloOA");
        var payload = new
        {
            phone,
            template_id = templateId,
            template_data = templateData
        };
        var response = await client.PostAsJsonAsync("/message/template", payload, ct);
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(ct);
            logger.LogError("Zalo OA API error. Status={Status} Body={Body}", response.StatusCode, body);
        }
    }
}
