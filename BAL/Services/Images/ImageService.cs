using System.Data;
using System.Security.Claims;
using DPCV_API.Configuration.DbContext;
using DPCV_API.Models.ImageModel.DPCV_API.Models.ImageModel;

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
        }

        public async Task<(bool Success, string Message, string? FilePath)> UploadMediaService(IFormFile file, EntityTypeEnum entityType, int entityId, ClaimsPrincipal user)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    _logger.LogWarning("Invalid file upload attempt.");
                    return (false, "Invalid file. Please upload a valid image.", null);
                }

                // Allowed file extensions
                var allowedExtensions = new HashSet<string> { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                string fileExtension = Path.GetExtension(file.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    _logger.LogWarning("File type not allowed: {FileExtension}", fileExtension);
                    return (false, "File type not supported. Please upload an image file.", null);
                }

                // Get MIME type and file size
                string mimeType = file.ContentType;
                long fileSize = file.Length;

                // **Get Committee ID and UploadedBy from Claims**
                int? committeeId = null;
                int? uploadedBy = null;

                if (user.Identity.IsAuthenticated)
                {
                    uploadedBy = int.TryParse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId) ? userId : (int?)null;
                }

                if (user.IsInRole("Committee"))
                {
                    var claimCommitteeId = user.FindFirst("committee_id")?.Value;
                    if (!int.TryParse(claimCommitteeId, out int parsedCommitteeId))
                    {
                        _logger.LogWarning("Committee user does not have a valid committee_id claim.");
                        return (false, "Unauthorized: No valid committee assignment found.", null);
                    }
                    committeeId = parsedCommitteeId;
                }

                // **Admin can upload images for any committee, but Committee users must match their own committee**
                if (committeeId.HasValue)
                {
                    _dataManager.ClearParameters();
                    _dataManager.AddParameter("@p_entity_id", entityId);
                    _dataManager.AddParameter("@p_entity_type", entityType.ToString());

                    var entityCommitteeId = await _dataManager.ExecuteScalarAsync<int>("GetEntityCommitteeIdForImage", CommandType.StoredProcedure);

                    if (entityCommitteeId != committeeId.Value)
                    {
                        _logger.LogWarning("Committee user {CommitteeId} tried to upload an image for entity {EntityId} but does not own it.", committeeId, entityId);
                        return (false, "Unauthorized: You can only upload images for your own entities.", null);
                    }
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

                // **Begin Transaction**
                _dataManager.BeginTransaction();
                try
                {
                    // Save file to server
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    _logger.LogInformation("File saved: {FilePath}", filePath);

                    // **Save Image Details to Database**
                    _dataManager.ClearParameters();
                    _dataManager.AddParameter("@p_image_url", relativeFilePath);
                    _dataManager.AddParameter("@p_original_image_name", file.FileName);
                    _dataManager.AddParameter("@p_image_name", uniqueFileName);
                    _dataManager.AddParameter("@p_file_size", fileSize);
                    _dataManager.AddParameter("@p_mime_type", mimeType);
                    _dataManager.AddParameter("@p_entity_type", entityType.ToString());
                    _dataManager.AddParameter("@p_entity_id", entityId);
                    _dataManager.AddParameter("@p_committee_id", committeeId.HasValue ? committeeId.Value : (object)DBNull.Value);
                    _dataManager.AddParameter("@p_uploaded_by", uploadedBy.HasValue ? uploadedBy.Value : (object)DBNull.Value);
                    _dataManager.AddParameter("@p_is_profile_image", 0);
                    _dataManager.AddParameter("@p_status", ImageStatusEnum.Active.ToString());

                    bool isInserted = await _dataManager.ExecuteNonQueryAsync("InsertImage", CommandType.StoredProcedure);

                    if (isInserted)
                    {
                        // **Commit Transaction**
                        _dataManager.CommitTransaction();
                        _logger.LogInformation("Image uploaded successfully: {FileName}, Entity: {EntityType}, ID: {EntityId}", file.FileName, entityType, entityId);
                        return (true, "Image uploaded successfully.", relativeFilePath);
                    }
                    else
                    {
                        _logger.LogWarning("Failed to insert image details into the database. Deleting the uploaded file.");
                        File.Delete(filePath); // Rollback file save if DB insert fails
                        _dataManager.RollbackTransaction();
                        return (false, "Failed to save image data. Please try again later.", null);
                    }
                }
                catch (Exception ex)
                {
                    // **Rollback Transaction on Exception**
                    _dataManager.RollbackTransaction();
                    _logger.LogError(ex, "Error uploading image: {FileName}", file.FileName);

                    // Delete the uploaded file if the transaction fails
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }

                    return (false, "An error occurred while uploading the image.", null);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in UploadMediaService");
                return (false, "An unexpected error occurred while uploading the image.", null);
            }
        }



        /// <summary>
        /// Retrieves a list of images associated with a given entity type and entity ID.
        /// </summary>
        /// <param name="entityType">The type of entity (e.g., Homestay, Activity, etc.).</param>
        /// <param name="entityId">The ID of the specific entity.</param>
        /// <returns>Returns a list of ImageDTO objects.</returns>
        public async Task<List<ImageDTO>?> GetImagesByEntityAsync(EntityTypeEnum entityType, int entityId)
        {
            _logger.LogInformation("Fetching images for EntityType: {EntityType}, EntityId: {EntityId}", entityType, entityId);

            _dataManager.ClearParameters();
            _dataManager.AddParameter("@p_entity_type", entityType.ToString());
            _dataManager.AddParameter("@p_entity_id", entityId);

            var images = new List<ImageDTO>();

            try
            {
                using (var reader = await _dataManager.ExecuteReaderAsync("GetImagesByEntity", CommandType.StoredProcedure))
                {
                    if (!reader.HasRows)
                    {
                        _logger.LogWarning("No images found for EntityType: {EntityType}, EntityId: {EntityId}", entityType, entityId);
                        return null; // Return null instead of empty list
                    }

                    while (await reader.ReadAsync())
                    {
                        _logger.LogInformation("Reading image row: ImageId={ImageId}", reader.GetInt32("image_id"));

                        images.Add(new ImageDTO
                        {
                            ImageId = reader.GetInt32("image_id"),
                            ImageUrl = reader.GetString("image_url"),
                            OriginalImageName = reader.IsDBNull("original_image_name") ? null : reader.GetString("original_image_name"),
                            ImageName = reader.IsDBNull("image_name") ? null : reader.GetString("image_name"),
                            FileSize = reader.IsDBNull("file_size") ? null : reader.GetInt64("file_size"),
                            MimeType = reader.IsDBNull("mime_type") ? null : reader.GetString("mime_type"),
                            EntityType = Enum.TryParse(reader.GetString("entity_type"), out EntityTypeEnum entityTypeEnum) ? entityTypeEnum : EntityTypeEnum.Product,
                            EntityId = reader.GetInt32("entity_id"),
                            CommitteeId = reader.IsDBNull("committee_id") ? null : reader.GetInt32("committee_id"),
                            UploadedBy = reader.IsDBNull("uploaded_by") ? null : reader.GetInt32("uploaded_by"),
                            IsProfileImage = reader.GetBoolean("is_profile_image"),
                            Status = Enum.TryParse(reader.GetString("status"), out ImageStatusEnum statusEnum) ? statusEnum : ImageStatusEnum.Active,
                            CreatedAt = reader.GetDateTime("created_at"),
                            UpdatedAt = reader.GetDateTime("updated_at")
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving images for EntityType: {EntityType}, EntityId: {EntityId}", entityType, entityId);
                return null; // Return null if an error occurs
            }

            _logger.LogInformation("Returning {Count} images for EntityType: {EntityType}, EntityId: {EntityId}", images.Count, entityType, entityId);
            return images;
        }

        public async Task<ImageDTO?> GetProfileImageByEntityAsync(EntityTypeEnum entityType, int entityId)
        {
            _logger.LogInformation("Fetching profile image for EntityType: {EntityType}, EntityId: {EntityId}", entityType, entityId);

            _dataManager.ClearParameters();
            _dataManager.AddParameter("@p_entity_type", entityType.ToString());
            _dataManager.AddParameter("@p_entity_id", entityId);

            try
            {
                using (var reader = await _dataManager.ExecuteReaderAsync("GetProfileImageByEntity", CommandType.StoredProcedure))
                {
                    if (await reader.ReadAsync()) // Read single record
                    {
                        _logger.LogInformation("Profile image found for EntityType: {EntityType}, EntityId: {EntityId}", entityType, entityId);

                        return new ImageDTO
                        {
                            ImageId = reader.GetInt32("image_id"),
                            ImageUrl = reader.GetString("image_url"),
                            OriginalImageName = reader.IsDBNull("original_image_name") ? null : reader.GetString("original_image_name"),
                            ImageName = reader.IsDBNull("image_name") ? null : reader.GetString("image_name"),
                            FileSize = reader.IsDBNull("file_size") ? null : reader.GetInt64("file_size"),
                            MimeType = reader.IsDBNull("mime_type") ? null : reader.GetString("mime_type"),
                            EntityType = Enum.TryParse(reader.GetString("entity_type"), out EntityTypeEnum entityTypeEnum) ? entityTypeEnum : EntityTypeEnum.Product,
                            EntityId = reader.GetInt32("entity_id"),
                            CommitteeId = reader.IsDBNull("committee_id") ? null : reader.GetInt32("committee_id"),
                            UploadedBy = reader.IsDBNull("uploaded_by") ? null : reader.GetInt32("uploaded_by"),
                            IsProfileImage = reader.GetBoolean("is_profile_image"),
                            Status = Enum.TryParse(reader.GetString("status"), out ImageStatusEnum statusEnum) ? statusEnum : ImageStatusEnum.Active,
                            CreatedAt = reader.GetDateTime("created_at"),
                            UpdatedAt = reader.GetDateTime("updated_at")
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving profile image for EntityType: {EntityType}, EntityId: {EntityId}", entityType, entityId);
            }

            _logger.LogWarning("No profile image found for EntityType: {EntityType}, EntityId: {EntityId}", entityType, entityId);
            return null; // No profile image found
        }

        public async Task<ImageDTO?> GetImageByIdAsync(int imageId)
        {
            _logger.LogInformation("Fetching image details: ImageId={ImageId}", imageId);

            _dataManager.ClearParameters();
            _dataManager.AddParameter("@p_image_id", imageId);

            try
            {
                using (var reader = await _dataManager.ExecuteReaderAsync("GetImageById", CommandType.StoredProcedure))
                {
                    if (await reader.ReadAsync())
                    {
                        _logger.LogInformation("Image found: ImageId={ImageId}", imageId);
                        return new ImageDTO
                        {
                            ImageId = reader.GetInt32("image_id"),
                            ImageUrl = reader.GetString("image_url"),
                            OriginalImageName = reader.IsDBNull("original_image_name") ? null : reader.GetString("original_image_name"),
                            ImageName = reader.IsDBNull("image_name") ? null : reader.GetString("image_name"),
                            FileSize = reader.IsDBNull("file_size") ? null : reader.GetInt64("file_size"),
                            MimeType = reader.IsDBNull("mime_type") ? null : reader.GetString("mime_type"),
                            EntityType = Enum.TryParse(reader.GetString("entity_type"), out EntityTypeEnum entityTypeEnum) ? entityTypeEnum : EntityTypeEnum.Product,
                            EntityId = reader.GetInt32("entity_id"),
                            CommitteeId = reader.IsDBNull("committee_id") ? null : reader.GetInt32("committee_id"),
                            UploadedBy = reader.IsDBNull("uploaded_by") ? null : reader.GetInt32("uploaded_by"),
                            IsProfileImage = reader.GetBoolean("is_profile_image"),
                            Status = Enum.TryParse(reader.GetString("status"), out ImageStatusEnum statusEnum) ? statusEnum : ImageStatusEnum.Active,
                            CreatedAt = reader.GetDateTime("created_at"),
                            UpdatedAt = reader.GetDateTime("updated_at")
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching image details: ImageId={ImageId}", imageId);
            }

            _logger.LogWarning("Image not found: ImageId={ImageId}", imageId);
            return null;
        }

        public async Task<(bool Success, string Message)> UpdateImageAsync(int imageId, IFormFile? newFile, ClaimsPrincipal user)
        {
            _logger.LogInformation("Updating image: ImageId={ImageId}", imageId);

            try
            {
                _dataManager.BeginTransaction(); // Start Transaction

                var userRole = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                var userCommitteeId = user.Claims.FirstOrDefault(c => c.Type == "committee_id")?.Value;

                var existingImage = await GetImageByIdAsync(imageId);
                if (existingImage == null)
                {
                    _dataManager.RollbackTransaction();
                    _logger.LogWarning("Image not found: ImageId={ImageId}", imageId);
                    return (false, "Image not found.");
                }

                if (userRole != "Admin" && existingImage.CommitteeId?.ToString() != userCommitteeId)
                {
                    _dataManager.RollbackTransaction();
                    _logger.LogWarning("Unauthorized attempt to update image: ImageId={ImageId}", imageId);
                    return (false, "Unauthorized.");
                }

                string imageUrl = existingImage.ImageUrl;
                string originalImageName = existingImage.OriginalImageName;
                string imageName = existingImage.ImageName;
                long? fileSize = existingImage.FileSize;
                string? mimeType = existingImage.MimeType;

                string uploadFolder = Path.Combine(_environment.WebRootPath, "Uploads");
                if (!Directory.Exists(uploadFolder))
                    Directory.CreateDirectory(uploadFolder);

                if (newFile != null)
                {
                    var allowedExtensions = new HashSet<string> { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                    string fileExtension = Path.GetExtension(newFile.FileName).ToLower();
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        _dataManager.RollbackTransaction();
                        _logger.LogWarning("File type not allowed: {FileExtension}", fileExtension);
                        return (false, "File type not supported.");
                    }

                    originalImageName = newFile.FileName;
                    imageName = $"{Guid.NewGuid()}{fileExtension}";
                    fileSize = newFile.Length;
                    mimeType = newFile.ContentType;
                    imageUrl = $"/Uploads/{imageName}";

                    string newFilePath = Path.Combine(uploadFolder, imageName);

                    // Save the file **only after DB update succeeds**
                    _logger.LogInformation("Updating image in database first...");

                    _dataManager.ClearParameters();
                    _dataManager.AddParameter("@p_image_id", imageId);
                    _dataManager.AddParameter("@p_new_image_url", imageUrl);
                    _dataManager.AddParameter("@p_original_image_name", originalImageName);
                    _dataManager.AddParameter("@p_image_name", imageName);
                    _dataManager.AddParameter("@p_file_size", fileSize);
                    _dataManager.AddParameter("@p_mime_type", mimeType);

                    var result = await _dataManager.ExecuteNonQueryAsync("UpdateImage", CommandType.StoredProcedure);

                    if (!result)
                    {
                        _dataManager.RollbackTransaction();
                        _logger.LogWarning("Failed to update image in database: ImageId={ImageId}", imageId);
                        return (false, "Failed to update image.");
                    }

                    // If DB update is successful, save the new image
                    using (var fileStream = new FileStream(newFilePath, FileMode.Create))
                    {
                        await newFile.CopyToAsync(fileStream);
                    }

                    _logger.LogInformation("New file uploaded: {FilePath}", newFilePath);

                    // Delete the old image file
                    if (!string.IsNullOrEmpty(existingImage.ImageUrl))
                    {
                        string oldFilePath = Path.Combine(_environment.WebRootPath, existingImage.ImageUrl.TrimStart('/'));
                        if (File.Exists(oldFilePath))
                        {
                            File.Delete(oldFilePath);
                            _logger.LogInformation("Deleted old file: {OldFilePath}", oldFilePath);
                        }
                    }

                    _dataManager.CommitTransaction();
                    return (true, "Image updated successfully.");
                }
                else
                {
                    _dataManager.RollbackTransaction();
                    return (false, "No new file provided.");
                }
            }
            catch (Exception ex)
            {
                _dataManager.RollbackTransaction();
                _logger.LogError(ex, "Error updating image: ImageId={ImageId}", imageId);
                return (false, "An error occurred while updating the image.");
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
