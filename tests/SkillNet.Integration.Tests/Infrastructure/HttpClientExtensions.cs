using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace SkillNet.Integration.Tests.Infrastructure;

public static partial class HttpClientExtensions
{
    [GeneratedRegex(@"<input[^>]+name=""__RequestVerificationToken""[^>]+value=""([^""]+)""", RegexOptions.IgnoreCase)]
    private static partial Regex AntiForgeryTokenRegex();

    public static async Task<string> GetAntiForgeryTokenAsync(this HttpClient client, string url)
    {
        var response = await client.GetAsync(url);
        var html = await response.Content.ReadAsStringAsync();

        var match = AntiForgeryTokenRegex().Match(html);
        if (!match.Success)
            throw new InvalidOperationException($"Anti-forgery token not found on page: {url}");

        return match.Groups[1].Value;
    }
}
