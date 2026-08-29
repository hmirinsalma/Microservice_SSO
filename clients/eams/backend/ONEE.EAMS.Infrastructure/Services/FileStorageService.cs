using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using ONEE.EAMS.Application.Common;
using ONEE.EAMS.Application.Interfaces;

namespace ONEE.EAMS.Infrastructure.Services;

public class FileStorageService : IFileStorageService
{
    private static readonly string[] AllowedDocExtensions = [".pdf", ".docx", ".xlsx", ".txt"];
    private static readonly string[] AllowedPhotoExtensions = [".jpg", ".jpeg", ".png", ".webp"];
    private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10 MB

    private readonly string _uploadRoot;

    public FileStorageService(IConfiguration config)
    {
        // Par défaut, on stocke dans wwwroot/uploads à côté de l'exécutable
        _uploadRoot = config["FileStorage:UploadRoot"]
            ?? Path.Combine(AppContext.BaseDirectory, "wwwroot", "uploads");
    }

    public async Task<string> UploadAsync(IFormFile file, string folder)
    {
        if (file.Length > MaxFileSizeBytes)
            throw new ValidationException(["Le fichier dépasse la taille maximale de 10 Mo."]);

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        var isPhoto = folder.Contains("photo");
        var allowed = isPhoto ? AllowedPhotoExtensions : AllowedDocExtensions;

        if (!allowed.Contains(ext))
            throw new ValidationException([$"Extension non autorisée. Extensions acceptées : {string.Join(", ", allowed)}"]);

        var uploadPath = Path.Combine(_uploadRoot, folder);
        Directory.CreateDirectory(uploadPath);

        var fileName = $"{Guid.NewGuid()}{ext}";
        var filePath = Path.Combine(uploadPath, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return $"/uploads/{folder}/{fileName}";
    }

    public Task DeleteAsync(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return Task.CompletedTask;
        var rel = url.Replace("/uploads/", "").Replace('/', Path.DirectorySeparatorChar);
        var fullPath = Path.Combine(_uploadRoot, rel);
        if (File.Exists(fullPath)) File.Delete(fullPath);
        return Task.CompletedTask;
    }
}
