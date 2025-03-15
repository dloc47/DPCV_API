using System.Data;
using System.Security.Claims;
using DPCV_API.Configuration.DbContext;
using DPCV_API.Models.ImageModel.DPCV_API.Models.ImageModel;
using static System.Net.Mime.MediaTypeNames;

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

        public async Task<(bool Success, string Message, string? FilePath)> UploadMediaService(
            IFormFile file, EntityTypeEnum entityType, int entityId, ClaimsPrincipal user)
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

                // **Extract User ID and Committee ID from Claims**
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

                // **Fetch entity's committee_id using stored procedure**
                _dataManager.ClearParameters();
                _dataManager.AddParameter("@p_entity_id", entityId);
                _dataManager.AddParameter("@p_entity_type", entityType.ToString());

                var entityCommitteeIdObject = await _dataManager.ExecuteScalarAsync<object>("GetEntityCommitteeIdForImage", CommandType.StoredProcedure);

                int? entityCommitteeId = entityCommitteeIdObject != null && int.TryParse(entityCommitteeIdObject.ToString(), out int tempId)
                    ? tempId
                    : (int?)null;

                // **Committee users can only upload images for their own entities**
                if (committeeId.HasValue && entityCommitteeId.HasValue && committeeId.Value != entityCommitteeId.Value)
                {
                    _logger.LogWarning("Committee user {CommitteeId} tried to upload an image for entity {EntityId} but does not own it.", committeeId, entityId);
                    return (false, "Unauthorized: You can only upload images for your own entities.", null);
                }

                // **Define Upload Directory**
                string uploadFolder = Path.Combine(_environment.WebRootPath, "Uploads");

                // **Ensure directory exists**
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                    _logger.LogInformation("Created directory: {UploadFolder}", uploadFolder);
                }

                // **Generate Unique File Name**
                string uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                string filePath = Path.Combine(uploadFolder, uniqueFileName);
                string relativeFilePath = $"/Uploads/{uniqueFileName}";

                // **Begin Transaction**
                _dataManager.BeginTransaction();
                try
                {
                    // **Save File to Server**
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    _logger.LogInformation("File saved: {FilePath}", filePath);

                    // **Insert Image Details into Database**
                    _dataManager.ClearParameters();
                    _dataManager.AddParameter("@p_image_url", relativeFilePath);
                    _dataManager.AddParameter("@p_original_image_name", file.FileName);
                    _dataManager.AddParameter("@p_image_name", uniqueFileName);
                    _dataManager.AddParameter("@p_file_size", fileSize);
                    _dataManager.AddParameter("@p_mime_type", mimeType);
                    _dataManager.AddParameter("@p_entity_type", entityType.ToString());
                    _dataManager.AddParameter("@p_entity_id", entityId);
                    _dataManager.AddParameter("@p_committee_id", entityCommitteeId.HasValue ? entityCommitteeId.Value : (object)DBNull.Value);
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
                        // **Rollback Transaction & Delete File if Insert Fails**
                        _logger.LogWarning("Failed to insert image details into the database. Deleting the uploaded file.");
                        File.Delete(filePath);
                        _dataManager.RollbackTransaction();
                        return (false, "Failed to save image data. Please try again later.", null);
                    }
                }
                catch (Exception ex)
                {
                    // **Rollback Transaction on Exception & Delete File**
                    _dataManager.RollbackTransaction();
                    _logger.LogError(ex, "Error uploading image: {FileName}", file.FileName);

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
                _logger.LogInformation("Transaction started successfully."); // Log confirmation

                var userRole = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                var userCommitteeId = user.Claims.FirstOrDefault(c => c.Type == "committee_id")?.Value;

                var existingImage = await GetImageByIdAsync(imageId);
                if (existingImage == null)
                {
                    _logger.LogWarning("Image not found: ImageId={ImageId}", imageId);
                    _dataManager.RollbackTransaction(); // Rollback only if transaction started
                    return (false, "Image not found.");
                }

                // Authorization: Only Admin (Role = "1") or the Committee owner can update the image
                if (userRole != "1" && existingImage.CommitteeId?.ToString() != userCommitteeId)
                {
                    _logger.LogWarning("Unauthorized attempt to update image: ImageId={ImageId}", imageId);
                    _dataManager.RollbackTransaction(); // Rollback only if transaction started
                    return (false, "Unauthorized.");
                }

                if (newFile == null)
                {
                    _logger.LogWarning("No new file provided for update: ImageId={ImageId}", imageId);
                    _dataManager.RollbackTransaction();
                    return (false, "No new file provided.");
                }

                string uploadFolder = Path.Combine(_environment.WebRootPath, "Uploads");
                if (!Directory.Exists(uploadFolder))
                    Directory.CreateDirectory(uploadFolder);

                var allowedExtensions = new HashSet<string> { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                string fileExtension = Path.GetExtension(newFile.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    _logger.LogWarning("File type not allowed: {FileExtension}", fileExtension);
                    _dataManager.RollbackTransaction();
                    return (false, "File type not supported.");
                }

                string imageName = $"{Guid.NewGuid()}{fileExtension}";
                string imageUrl = $"/Uploads/{imageName}";
                string newFilePath = Path.Combine(uploadFolder, imageName);

                _dataManager.ClearParameters();
                _dataManager.AddParameter("@p_image_id", imageId);
                _dataManager.AddParameter("@p_new_image_url", imageUrl);
                _dataManager.AddParameter("@p_original_image_name", newFile.FileName);
                _dataManager.AddParameter("@p_image_name", imageName);
                _dataManager.AddParameter("@p_file_size", newFile.Length);
                _dataManager.AddParameter("@p_mime_type", newFile.ContentType);

                var result = await _dataManager.ExecuteNonQueryAsync("UpdateImage", CommandType.StoredProcedure);
                if (!result)
                {
                    _logger.LogWarning("Failed to update image in database: ImageId={ImageId}", imageId);
                    _dataManager.RollbackTransaction();
                    return (false, "Failed to update image.");
                }

                // Save the file **only after DB update succeeds**
                using (var fileStream = new FileStream(newFilePath, FileMode.Create))
                {
                    await newFile.CopyToAsync(fileStream);
                }

                // Delete old image
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
                _logger.LogInformation("Transaction committed successfully.");
                return (true, "Image updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating image: ImageId={ImageId}", imageId);

                // Ensure rollback only if transaction was started
                try
                {
                    _dataManager.RollbackTransaction();
                }
                catch (Exception rollbackEx)
                {
                    _logger.LogError(rollbackEx, "Rollback transaction failed: ImageId={ImageId}", imageId);
                }

                return (false, "An error occurred while updating the image.");
            }
        }


        public async Task<(bool Success, string Message)> DeleteImageAsync(int imageId, ClaimsPrincipal user)
        {
            _logger.LogInformation("Deleting image: ImageId={ImageId}", imageId);

            try
            {
                // Start a new transaction
                _dataManager.BeginTransaction();
                _logger.LogInformation("Transaction started successfully.");

                // Fetch the existing image details
                var existingImage = await GetImageByIdAsync(imageId);
                if (existingImage == null)
                {
                    _logger.LogWarning("Image not found: ImageId={ImageId}", imageId);
                    _dataManager.RollbackTransaction(); // Rollback if image not found
                    return (false, "Image not found.");
                }

                // Authorization: Only Admin (Role = "1") or the Committee owner can delete the profile image
                var userRole = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                var userCommitteeId = user.Claims.FirstOrDefault(c => c.Type == "committee_id")?.Value;

                if (userRole != "1" && existingImage.CommitteeId?.ToString() != userCommitteeId)
                {
                    _logger.LogWarning("Unauthorized attempt to delete image: ImageId={ImageId}", imageId);
                    _dataManager.RollbackTransaction(); // Rollback if unauthorized
                    return (false, "Unauthorized.");
                }

                // Delete the image record from the database
                _dataManager.ClearParameters();
                _dataManager.AddParameter("@p_image_id", imageId);

                bool deleteResult = await _dataManager.ExecuteNonQueryAsync("DeleteImage", CommandType.StoredProcedure);
                if (!deleteResult)
                {
                    _logger.LogWarning("Failed to delete image from database: ImageId={ImageId}", imageId);
                    _dataManager.RollbackTransaction(); // Rollback if database deletion fails
                    return (false, "Failed to delete image.");
                }

                // Delete the associated image file from the file system (wwwroot/Uploads)
                if (!string.IsNullOrEmpty(existingImage.ImageUrl))
                {
                    string filePath = Path.Combine(_environment.WebRootPath, existingImage.ImageUrl.TrimStart('/'));
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                        _logger.LogInformation("Deleted image file: {FilePath}", filePath);
                    }
                    else
                    {
                        _logger.LogWarning("Image file not found: {FilePath}", filePath);
                    }
                }

                // Commit the transaction if everything succeeds
                _dataManager.CommitTransaction();
                _logger.LogInformation("Transaction committed successfully.");
                return (true, "Image deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting image: ImageId={ImageId}", imageId);

                // Rollback the transaction in case of an error
                try
                {
                    _dataManager.RollbackTransaction();
                }
                catch (Exception rollbackEx)
                {
                    _logger.LogError(rollbackEx, "Rollback transaction failed: ImageId={ImageId}", imageId);
                }

                return (false, "An error occurred while deleting the image.");
            }
        }

        public async Task<(bool Success, string Message)> SetProfileImageAsync(int imageId, ClaimsPrincipal user)
        {
            _logger.LogInformation("Setting profile image: ImageId={ImageId}", imageId);

            try
            {
                // Fetch the image details first
                var image = await GetImageByIdAsync(imageId);
                if (image == null)
                {
                    _logger.LogWarning("Image not found: ImageId={ImageId}", imageId);
                    return (false, "Image not found.");
                }

                // Extract user role and committee ID from claims
                var userRole = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                var userCommitteeId = user.Claims.FirstOrDefault(c => c.Type == "committee_id")?.Value;

                // Authorization: Only Admin (Role = "1") or the Committee owner can update the profile image
                if (userRole != "1" && (image.CommitteeId == null || image.CommitteeId.ToString() != userCommitteeId))
                {
                    _logger.LogWarning("Unauthorized attempt to set profile image: ImageId={ImageId}", imageId);
                    return (false, "Unauthorized.");
                }

                // Call stored procedure to update profile image
                _dataManager.ClearParameters();
                _dataManager.AddParameter("@p_image_id", imageId);

                bool updateResult = await _dataManager.ExecuteNonQueryAsync("SetProfileImage", CommandType.StoredProcedure);
                if (!updateResult)
                {
                    _logger.LogWarning("Failed to set profile image: ImageId={ImageId}", imageId);
                    return (false, "Failed to update profile image.");
                }

                _logger.LogInformation("Profile image updated successfully: ImageId={ImageId}", imageId);
                return (true, "Profile image updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting profile image: ImageId={ImageId}", imageId);
                return (false, "An error occurred while setting the profile image.");
            }
        }


    }
}
