//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.IO;
//using System.Threading.Tasks;

//[Route("api/[controller]")]
//[ApiController]
//public class FileUploadController : ControllerBase
//{
//    private readonly IWebHostEnvironment _environment;

//    public FileUploadController(IWebHostEnvironment environment)
//    {
//        _environment = environment;
//    }

//    [HttpPost("upload")]
//    [Consumes("multipart/form-data")] // ✅ REQUIRED for Swagger to work with IFormFile
//    public async Task<IActionResult> UploadDocument([FromForm] IFormFile file)
//    {
//        try
//        {
//            if (file == null || file.Length == 0)
//                return BadRequest(new { message = "Invalid file. Please upload a valid document." });

//            // Ensure upload directory exists
//            string uploadFolder = Path.Combine(_environment.WebRootPath, "uploads");
//            if (!Directory.Exists(uploadFolder))
//                Directory.CreateDirectory(uploadFolder);

//            // Generate unique filename
//            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
//            string filePath = Path.Combine(uploadFolder, fileName);

//            // Save file to server
//            using (var stream = new FileStream(filePath, FileMode.Create))
//            {
//                await file.CopyToAsync(stream);
//            }

//            // Return file URL
//            string fileUrl = $"/uploads/{fileName}";
//            return Ok(new { message = "File uploaded successfully.", fileUrl });
//        }
//        catch (Exception ex)
//        {
//            return StatusCode(500, new { message = "An error occurred while uploading the file.", error = ex.Message });
//        }
//    }
//}
