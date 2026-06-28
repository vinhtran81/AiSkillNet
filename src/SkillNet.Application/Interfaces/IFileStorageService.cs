using Microsoft.AspNetCore.Http;

namespace SkillNet.Application.Interfaces;

public interface IFileStorageService
{
    Task<string> UploadAsync(IFormFile file, string folder, CancellationToken ct = default);
    Task DeleteAsync(string filePath, CancellationToken ct = default);
}
