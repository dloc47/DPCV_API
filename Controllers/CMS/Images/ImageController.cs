using DPCV_API.BAL.Services.Images;
using Microsoft.AspNetCore.Mvc;
using DPCV_API.Models.ImageModel;
using Microsoft.AspNetCore.Authorization;
using DPCV_API.Models.ImageModel.DPCV_API.Models.ImageModel;

namespace DPCV_API.Controllers.CMS.Images
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;

        public ImageController(IImageService imageService)
        {
            _imageService = imageService;
        }

        [Authorize]
        [HttpPost("upload")]
        public async Task<IActionResult> UploadMedia([FromForm] UploadMediaRequest request)
        {
            if (request.File == null || request.File.Length == 0)
            {
                return BadRequest(new { success = false, message = "Invalid file. Please upload a valid image." });
            }

            var user = HttpContext.User;
            var result = await _imageService.UploadMediaService(request.File, request.EntityType, request.EntityId, user);

            if (result.Success)
            {
                return Ok(new { success = true, message = result.Message, imageUrl = result.FilePath });
            }

            return BadRequest(new { success = false, message = result.Message });
        }

        [HttpGet("{entityType}/{entityId}/images")]
        public async Task<IActionResult> GetImagesByEntity(EntityTypeEnum entityType, int entityId)
        {
            var images = await _imageService.GetImagesByEntityAsync(entityType, entityId);

            if (images == null || images.Count == 0)
                return NotFound(new { message = "No images found for the specified entity." });

            return Ok(images);
        }

        [HttpGet("{entityType}/{entityId}/profile-image")]
        public async Task<IActionResult> GetProfileImageByEntity(EntityTypeEnum entityType, int entityId)
        {
            var image = await _imageService.GetProfileImageByEntityAsync(entityType, entityId);

            if (image == null)
                return NotFound(new { message = "No profile image found for the specified entity." });

            return Ok(image);
        }


        [Authorize]
        [HttpPut("{imageId}")]
        public async Task<IActionResult> UpdateImage(int imageId, IFormFile? newFile, string? newName)
        {
            var user = HttpContext.User;
            var success = await _imageService.UpdateImageAsync(imageId, newFile, newName, user);
            if (success)
                return NoContent();
            return BadRequest();
        }

        [Authorize]
        [HttpDelete("{imageId}")]
        public async Task<IActionResult> DeleteImage(int imageId)
        {
            var user = HttpContext.User;
            var success = await _imageService.DeleteImageAsync(imageId, user);
            if (success)
                return NoContent();
            return BadRequest();
        }

        [Authorize]
        [HttpPost("{imageId}/set-profile")]
        public async Task<IActionResult> SetProfileImage(int imageId)
        {
            var user = HttpContext.User;
            var success = await _imageService.SetProfileImageAsync(imageId, user);
            if (success)
                return NoContent();
            return BadRequest();
        }
    }
}
