using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Claims;
using DPCV_API.Configuration.DbContext;

namespace DPCV_API.BAL.Services.Images
{
    public class ImageService : IImageService
    {
        private readonly DataManager _dataManager;
        private readonly ILogger<ImageService> _logger;
        private readonly IWebHostEnvironment _environment;

        public ImageService(DataManager dataManager, ILogger<ImageService> logger, IWebHostEnvironment environment)
        {
            _dataManager = dataManager;
            _logger = logger;
            _environment = environment;

            _logger.LogInformation("ImageService initialized successfully.");

            //// Ensure WebRootPath is not null
            //if (string.IsNullOrEmpty(_environment.WebRootPath))
            //{
            //    _environment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            //}
        }

        public async Task<(bool Success, string Message, string? FilePath)> UploadMediaService(IFormFile document, string entityType, int entityId, ClaimsPrincipal user)
        {
            try
            {
                if (document == null || document.Length == 0)
                {
                    _logger.LogWarning("Invalid file upload attempt.");
                    return (false, "Invalid file. Please upload a valid image.", null);
                }

                // Allowed file extensions
                var allowedExtensions = new HashSet<string> { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                string fileExtension = Path.GetExtension(document.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    _logger.LogWarning("File type not allowed: {FileExtension}", fileExtension);
                    return (false, "File type not supported. Please upload an image file.", null);
                }

                // Define upload directory inside wwwroot
                string uploadFolder = Path.Combine(_environment.WebRootPath, "Uploads");

                // Ensure directory exists
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                    _logger.LogInformation("Created directory: {UploadFolder}", uploadFolder);
                }

                // Generate unique file name
                string uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                string filePath = Path.Combine(uploadFolder, uniqueFileName);
                string relativeFilePath = $"/Uploads/{uniqueFileName}";

                // Save file to server
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await document.CopyToAsync(fileStream);
                }

                _logger.LogInformation("File saved: {FilePath}", filePath);

                // Save image details to database
                _dataManager.ClearParameters();
                _dataManager.AddParameter("@p_image_url", relativeFilePath);
                _dataManager.AddParameter("@p_image_name", document.FileName);
                _dataManager.AddParameter("@p_entity_type", entityType);
                _dataManager.AddParameter("@p_entity_id", entityId);
                _dataManager.AddParameter("@p_is_profile_image", 0);

                bool isInserted = await _dataManager.ExecuteNonQueryAsync("InsertImage", CommandType.StoredProcedure);

                if (isInserted)
                {
                    _logger.LogInformation("Image uploaded successfully: {FileName}, Entity: {EntityType}, ID: {EntityId}", document.FileName, entityType, entityId);
                    return (true, "Image uploaded successfully.", relativeFilePath);
                }
                else
                {
                    _logger.LogWarning("Failed to insert image details into the database. Deleting the uploaded file.");
                    File.Delete(filePath); // Rollback file save if DB insert fails
                    return (false, "Failed to save image data. Please try again later.", null);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image: {FileName}", document.FileName);
                return (false, "An error occurred while uploading the image.", null);
            }
        }


        public async Task<List<string>> GetImagesByEntityAsync(string entityType, int entityId)
        {
            _dataManager.ClearParameters();
            _dataManager.AddParameter("@p_entity_type", entityType);
            _dataManager.AddParameter("@p_entity_id", entityId);

            var images = new List<string>();
            using (var reader = await _dataManager.ExecuteReaderAsync("GetImagesByEntity", CommandType.StoredProcedure))
            {
                while (reader.Read())
                {
                    images.Add(reader.GetString("image_url"));
                }
            }
            return images;
        }

        public async Task<string?> GetProfileImageByEntityAsync(string entityType, int entityId)
        {
            _dataManager.ClearParameters();
            _dataManager.AddParameter("@p_entity_type", entityType);
            _dataManager.AddParameter("@p_entity_id", entityId);

            return await _dataManager.ExecuteScalarAsync<string>("GetProfileImageByEntity", CommandType.StoredProcedure);
        }

        public async Task<bool> UpdateImageAsync(int imageId, IFormFile? newFile, string? newName, ClaimsPrincipal user)
        {
            try
            {
                _dataManager.ClearParameters();
                _dataManager.AddParameter("@p_image_id", imageId);
                _dataManager.AddParameter("@p_new_name", newName);
                return await _dataManager.ExecuteNonQueryAsync("UpdateImage", CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating image");
                return false;
            }
        }

        public async Task<bool> DeleteImageAsync(int imageId, ClaimsPrincipal user)
        {
            try
            {
                _dataManager.ClearParameters();
                _dataManager.AddParameter("@p_image_id", imageId);
                return await _dataManager.ExecuteNonQueryAsync("DeleteImage", CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting image");
                return false;
            }
        }

        public async Task<bool> SetProfileImageAsync(int imageId, ClaimsPrincipal user)
        {
            try
            {
                _dataManager.ClearParameters();
                _dataManager.AddParameter("@p_image_id", imageId);
                return await _dataManager.ExecuteNonQueryAsync("SetProfileImage", CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting profile image");
                return false;
            }
        }

    }
}
