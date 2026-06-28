using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using SkillNet.Application.Interfaces;

namespace SkillNet.Infrastructure.Services;

public class LocalFileStorageService(IWebHostEnvironment env) : IFileStorageService
{
    public async Task<string> UploadAsync(IFormFile file, string folder, CancellationToken ct = default)
    {
        var uploadDir = Path.Combine(env.WebRootPath, "uploads", folder);
        Directory.CreateDirectory(uploadDir);

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(uploadDir, fileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream, ct);

        return $"/uploads/{folder}/{fileName}";
    }

    public Task DeleteAsync(string filePath, CancellationToken ct = default)
    {
        var fullPath = Path.Combine(env.WebRootPath, filePath.TrimStart('/'));
        if (File.Exists(fullPath)) File.Delete(fullPath);
        return Task.CompletedTask;
    }
}
