﻿using System.Security.Claims;
using DPCV_API.Models.ImageModel.DPCV_API.Models.ImageModel;

namespace DPCV_API.BAL.Services.Images
{
    public interface IImageService
    {
        Task<(bool Success, string Message, string? FilePath)> UploadMediaService(IFormFile file, EntityTypeEnum entityType, int entityId, ClaimsPrincipal user);
        Task<List<ImageDTO>?> GetImagesByEntityAsync(EntityTypeEnum entityType, int entityId);
        Task<ImageDTO?> GetProfileImageByEntityAsync(EntityTypeEnum entityType, int entityId);
        Task<ImageDTO?> GetImageByIdAsync(int imageId);
        Task<(bool Success, string Message)> UpdateImageAsync(int imageId, IFormFile? newFile, ClaimsPrincipal user);
        Task<(bool Success, string Message)> DeleteImageAsync(int imageId, ClaimsPrincipal user);
        Task<(bool Success, string Message)> SetProfileImageAsync(int imageId, ClaimsPrincipal user);
    }
}
