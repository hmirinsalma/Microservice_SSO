using Microsoft.AspNetCore.Http;

namespace ONEE.EAMS.Application.Interfaces;

public interface IFileStorageService
{
    Task<string> UploadAsync(IFormFile file, string folder);
    Task DeleteAsync(string url);
}
