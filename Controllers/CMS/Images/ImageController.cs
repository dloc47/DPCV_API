using DPCV_API.BAL.Services.Images;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost("upload")]
        public async Task<IActionResult> UploadMedia(IFormFile document, string entityType, int entityId)
        {
            var user = HttpContext.User;
            var result = await _imageService.UploadMediaService(document, entityType, entityId, user);
            if (result.Success)
                return Ok(result.Message);
            return BadRequest(result.Message);
        }

        [HttpGet("{entityType}/{entityId}/images")]
        public async Task<IActionResult> GetImagesByEntity(string entityType, int entityId)
        {
            var images = await _imageService.GetImagesByEntityAsync(entityType, entityId);
            return Ok(images);
        }

        [HttpGet("{entityType}/{entityId}/profile-image")]
        public async Task<IActionResult> GetProfileImageByEntity(string entityType, int entityId)
        {
            var image = await _imageService.GetProfileImageByEntityAsync(entityType, entityId);
            if (image != null)
                return Ok(image);
            return NotFound();
        }

        [HttpPut("{imageId}")]
        public async Task<IActionResult> UpdateImage(int imageId, IFormFile? newFile, string? newName)
        {
            var user = HttpContext.User;
            var success = await _imageService.UpdateImageAsync(imageId, newFile, newName, user);
            if (success)
                return NoContent();
            return BadRequest();
        }

        [HttpDelete("{imageId}")]
        public async Task<IActionResult> DeleteImage(int imageId)
        {
            var user = HttpContext.User;
            var success = await _imageService.DeleteImageAsync(imageId, user);
            if (success)
                return NoContent();
            return BadRequest();
        }

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
