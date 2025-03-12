using System.Security.Claims;
using DPCV_API.Models.ImageModel.DPCV_API.Models.ImageModel;
using MySql.Data.MySqlClient;

namespace DPCV_API.BAL.Services.Images
{
    public interface IImageService
    {
        Task<(bool Success, string Message, string? FilePath)> UploadMediaService(IFormFile file, EntityTypeEnum entityType, int entityId, ClaimsPrincipal user);
        Task<List<ImageDTO>> GetImagesByEntityAsync(EntityTypeEnum entityType, int entityId);
        Task<ImageDTO?> GetProfileImageByEntityAsync(EntityTypeEnum entityType, int entityId);
        Task<bool> UpdateImageAsync(int imageId, IFormFile? newFile, string? newName, ClaimsPrincipal user);
        Task<bool> DeleteImageAsync(int imageId, ClaimsPrincipal user);
        Task<bool> SetProfileImageAsync(int imageId, ClaimsPrincipal user);
    }
}
