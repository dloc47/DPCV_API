using System.Security.Claims;
using MySql.Data.MySqlClient;

namespace DPCV_API.BAL.Services.Images
{
    public interface IImageService
    {
        Task<(bool Success, string Message, string? FilePath)> UploadMediaService(IFormFile document, string entityType, int entityId, ClaimsPrincipal user);
        Task<List<string>> GetImagesByEntityAsync(string entityType, int entityId);
        Task<string?> GetProfileImageByEntityAsync(string entityType, int entityId);
        Task<bool> UpdateImageAsync(int imageId, IFormFile? newFile, string? newName, ClaimsPrincipal user);
        Task<bool> DeleteImageAsync(int imageId, ClaimsPrincipal user);
        Task<bool> SetProfileImageAsync(int imageId, ClaimsPrincipal user);
    }
}
