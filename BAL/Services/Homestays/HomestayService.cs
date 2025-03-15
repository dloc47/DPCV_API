using System.Security.Claims;
using DPCV_API.Configuration.DbContext;
using System.Data;
using System.Text.Json;
using DPCV_API.Models.HomestayModel;
using DPCV_API.Helpers;
using DPCV_API.Models.CommonModel;

namespace DPCV_API.BAL.Services.Homestays
{
    public class HomestayService : IHomestayService
    {
        private readonly DataManager _dataManager;
        private readonly ILogger<HomestayService> _logger;

        public HomestayService(DataManager dataManager, ILogger<HomestayService> logger)
        {
            _dataManager = dataManager;
            _logger = logger;
        }


        public async Task<PaginatedResponse<HomestayResponseDTO>> GetPaginatedHomestaysAsync(int pageNumber, int pageSize)
        {
            string procedureName = "GetPaginatedHomestays";

            var parameters = new Dictionary<string, object>
    {
        { "PageNumber", pageNumber },
        { "PageSize", pageSize }
    };

            DataTable result = await _dataManager.ExecuteQueryAsync(procedureName, CommandType.StoredProcedure, parameters);
            List<HomestayResponseDTO> homestays = new();

            foreach (DataRow row in result.Rows)
            {
                homestays.Add(new HomestayResponseDTO
                {
                    HomestayId = Convert.ToInt32(row["homestay_id"]),
                    HomestayName = row["homestay_name"].ToString()!,
                    CommitteeId = Convert.ToInt32(row["committee_id"]),
                    Address = row["address"].ToString()!,
                    Description = row["description"] != DBNull.Value ? row["description"].ToString()! : string.Empty,
                    OwnerName = row["owner_name"].ToString()!,
                    OwnerMobile = row["owner_mobile"].ToString()!,
                    TotalRooms = Convert.ToInt32(row["total_rooms"]),
                    RoomTariff = Convert.ToDecimal(row["room_tariff"]),
                    Tags = JsonHelper.DeserializeJsonSafely<List<string>>(row["tags"], "tags"),
                    Amenities = JsonHelper.DeserializeJsonSafely<List<string>>(row["amenities"], "amenities"),
                    PaymentMethods = row["payment_methods"] != DBNull.Value ? row["payment_methods"].ToString()! : string.Empty,
                    SocialMediaLinks = JsonHelper.DeserializeJsonSafely<Dictionary<string, string>>(row["social_media_links"], "social_media_links"),
                    IsVerifiable = Convert.ToBoolean(row["isVerifiable"]),
                    VerificationStatusId = row["verification_status_id"] != DBNull.Value ? Convert.ToInt32(row["verification_status_id"]) : null,
                    IsActive = Convert.ToInt32(row["is_active"]),
                    DistrictId = Convert.ToInt32(row["district_id"]),
                    DistrictName = row["district_name"].ToString()!,
                    Region = row["region"].ToString()!,
                    StatusType = row["status_type"] != DBNull.Value ? row["status_type"].ToString()! : null
                });
            }

            return new PaginatedResponse<HomestayResponseDTO>
            {
                Data = homestays,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = await GetTotalHomestayCountAsync()
            };
        }

        // ✅ Get total homestay count
        private async Task<int> GetTotalHomestayCountAsync()
        {
            string query = "SELECT COUNT(*) FROM homestays";

            object? result = await _dataManager.ExecuteScalarAsync<object>(query, CommandType.Text);

            return result != null ? Convert.ToInt32(result) : 0;
        }


        public async Task<List<HomestayResponseDTO>> GetAllHomestaysAsync()
        {
            string spName = "GetAllHomestays";
            List<HomestayResponseDTO> homestays = new();

            try
            {
                DataTable result = await _dataManager.ExecuteQueryAsync(spName, CommandType.StoredProcedure);

                foreach (DataRow row in result.Rows)
                {
                    homestays.Add(new HomestayResponseDTO
                    {
                        HomestayId = Convert.ToInt32(row["homestay_id"]),
                        HomestayName = row["homestay_name"].ToString()!,
                        CommitteeId = Convert.ToInt32(row["committee_id"]),
                        Address = row["address"].ToString()!,
                        Description = row["description"] != DBNull.Value ? row["description"].ToString()! : string.Empty, // New Field
                        OwnerName = row["owner_name"].ToString()!,
                        OwnerMobile = row["owner_mobile"].ToString()!,
                        TotalRooms = Convert.ToInt32(row["total_rooms"]),
                        RoomTariff = Convert.ToDecimal(row["room_tariff"]),
                        Tags = JsonHelper.DeserializeJsonSafely<List<string>>(row["tags"], "tags"),
                        Amenities = JsonHelper.DeserializeJsonSafely<List<string>>(row["amenities"], "amenities"), // New Field
                        PaymentMethods = row["payment_methods"] != DBNull.Value ? row["payment_methods"].ToString()! : string.Empty, // New Field
                        SocialMediaLinks = JsonHelper.DeserializeJsonSafely<Dictionary<string, string>>(row["social_media_links"], "social_media_links"), // New Field
                        IsVerifiable = Convert.ToBoolean(row["isVerifiable"]),
                        VerificationStatusId = row["verification_status_id"] != DBNull.Value ? Convert.ToInt32(row["verification_status_id"]) : null,
                        IsActive = Convert.ToInt32(row["is_active"]),
                        DistrictId = Convert.ToInt32(row["district_id"]),
                        DistrictName = row["district_name"].ToString()!,
                        Region = row["region"].ToString()!,
                        StatusType = row["status_type"] != DBNull.Value ? row["status_type"].ToString()! : null
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching homestays.");
                throw;
            }

            return homestays;
        }

        public async Task<HomestayResponseDTO?> GetHomestayByIdAsync(int homestayId)
        {
            string spName = "GetHomestayById";

            try
            {
                _dataManager.ClearParameters();
                _dataManager.AddParameter("@p_homestay_id", homestayId);
                DataTable result = await _dataManager.ExecuteQueryAsync(spName, CommandType.StoredProcedure);

                if (result.Rows.Count == 0)
                {
                    _logger.LogWarning($"Homestay with ID {homestayId} not found.");
                    return null;
                }

                DataRow row = result.Rows[0];
                return new HomestayResponseDTO
                {
                    HomestayId = Convert.ToInt32(row["homestay_id"]),
                    HomestayName = row["homestay_name"].ToString()!,
                    CommitteeId = Convert.ToInt32(row["committee_id"]),
                    Address = row["address"].ToString()!,
                    Description = row["description"] != DBNull.Value ? row["description"].ToString()! : string.Empty, // New Field
                    OwnerName = row["owner_name"].ToString()!,
                    OwnerMobile = row["owner_mobile"].ToString()!,
                    TotalRooms = Convert.ToInt32(row["total_rooms"]),
                    RoomTariff = Convert.ToDecimal(row["room_tariff"]),
                    Tags = JsonHelper.DeserializeJsonSafely<List<string>>(row["tags"], "tags"),
                    Amenities = JsonHelper.DeserializeJsonSafely<List<string>>(row["amenities"], "amenities"), // New Field
                    PaymentMethods = row["payment_methods"] != DBNull.Value ? row["payment_methods"].ToString()! : string.Empty, // New Field
                    SocialMediaLinks = JsonHelper.DeserializeJsonSafely<Dictionary<string, string>>(row["social_media_links"], "social_media_links"), // New Field
                    IsVerifiable = Convert.ToBoolean(row["isVerifiable"]),
                    VerificationStatusId = row["verification_status_id"] != DBNull.Value ? Convert.ToInt32(row["verification_status_id"]) : null,
                    IsActive = Convert.ToInt32(row["is_active"]),
                    DistrictId = Convert.ToInt32(row["district_id"]),
                    DistrictName = row["district_name"].ToString()!,
                    Region = row["region"].ToString()!,
                    StatusType = row["status_type"] != DBNull.Value ? row["status_type"].ToString()! : null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching homestay with ID {homestayId}.");
                throw;
            }
        }


        public async Task<bool> CreateHomestayAsync(HomestayDTO homestay, ClaimsPrincipal user)
        {
            try
            {
                var roleClaim = user.FindFirst(ClaimTypes.Role);
                var committeeClaim = user.FindFirst("CommitteeId");

                int roleId = roleClaim != null ? int.Parse(roleClaim.Value) : 0;
                int? committeeId = committeeClaim != null ? int.Parse(committeeClaim.Value) : null;

                if (roleId == 2 && committeeId != homestay.CommitteeId)
                {
                    _logger.LogWarning("Unauthorized attempt to create a homestay.");
                    return false;
                }

                int verificationStatus = roleId == 1 ? 2 : 1;

                string procedureName = "CreateHomestay";
                _dataManager.ClearParameters();
                _dataManager.AddParameter("@p_homestay_name", homestay.HomestayName);
                _dataManager.AddParameter("@p_committee_id", homestay.CommitteeId);
                _dataManager.AddParameter("@p_address", homestay.Address);
                _dataManager.AddParameter("@p_description", homestay.Description); // New Field
                _dataManager.AddParameter("@p_owner_name", homestay.OwnerName);
                _dataManager.AddParameter("@p_owner_mobile", homestay.OwnerMobile);
                _dataManager.AddParameter("@p_total_rooms", homestay.TotalRooms);
                _dataManager.AddParameter("@p_room_tariff", homestay.RoomTariff);
                _dataManager.AddParameter("@p_tags", JsonSerializer.Serialize(homestay.Tags));
                _dataManager.AddParameter("@p_amenities", JsonSerializer.Serialize(homestay.Amenities)); // New Field
                _dataManager.AddParameter("@p_payment_methods", homestay.PaymentMethods); // New Field
                _dataManager.AddParameter("@p_social_media_links", JsonSerializer.Serialize(homestay.SocialMediaLinks)); // New Field
                _dataManager.AddParameter("@p_isVerifiable", homestay.IsVerifiable);
                _dataManager.AddParameter("@p_verification_status_id", verificationStatus);
                _dataManager.AddParameter("@p_is_active", homestay.IsActive);

                return await _dataManager.ExecuteNonQueryAsync(procedureName, CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating homestay.");
                return false;
            }
        }

        public async Task<bool> UpdateHomestayAsync(HomestayDTO homestay, ClaimsPrincipal user)
        {
            try
            {
                var roleClaim = user.FindFirst(ClaimTypes.Role);
                var committeeClaim = user.FindFirst("CommitteeId");

                int roleId = roleClaim != null ? int.Parse(roleClaim.Value) : 0;
                int? userCommitteeId = committeeClaim != null ? int.Parse(committeeClaim.Value) : null;

                // Fetch existing homestay details
                _dataManager.ClearParameters();
                _dataManager.AddParameter("@p_homestay_id", homestay.HomestayId);
                DataTable dt = await _dataManager.ExecuteQueryAsync("GetHomestayById", CommandType.StoredProcedure);

                if (dt.Rows.Count == 0)
                    return false;

                int existingCommitteeId = Convert.ToInt32(dt.Rows[0]["committee_id"]);
                if (roleId == 2 && existingCommitteeId != userCommitteeId)
                {
                    _logger.LogWarning("Unauthorized attempt to update homestay: {HomestayId}", homestay.HomestayId);
                    return false;
                }

                int verificationStatus = roleId == 1 ? 2 : 1;

                string procedureName = "UpdateHomestay";
                _dataManager.ClearParameters();
                _dataManager.AddParameter("@p_homestay_id", homestay.HomestayId);
                _dataManager.AddParameter("@p_homestay_name", homestay.HomestayName);
                _dataManager.AddParameter("@p_committee_id", homestay.CommitteeId);
                _dataManager.AddParameter("@p_address", homestay.Address);
                _dataManager.AddParameter("@p_description", homestay.Description); // New Field
                _dataManager.AddParameter("@p_owner_name", homestay.OwnerName);
                _dataManager.AddParameter("@p_owner_mobile", homestay.OwnerMobile);
                _dataManager.AddParameter("@p_total_rooms", homestay.TotalRooms);
                _dataManager.AddParameter("@p_room_tariff", homestay.RoomTariff);
                _dataManager.AddParameter("@p_tags", JsonSerializer.Serialize(homestay.Tags));
                _dataManager.AddParameter("@p_amenities", JsonSerializer.Serialize(homestay.Amenities)); // New Field
                _dataManager.AddParameter("@p_payment_methods", homestay.PaymentMethods); // New Field
                _dataManager.AddParameter("@p_social_media_links", JsonSerializer.Serialize(homestay.SocialMediaLinks)); // New Field
                _dataManager.AddParameter("@p_isVerifiable", homestay.IsVerifiable);
                _dataManager.AddParameter("@p_verification_status_id", verificationStatus);
                _dataManager.AddParameter("@p_is_active", homestay.IsActive);

                bool success = await _dataManager.ExecuteNonQueryAsync(procedureName, CommandType.StoredProcedure);
                if (success)
                    _logger.LogInformation("Homestay updated successfully: {HomestayId}", homestay.HomestayId);
                else
                    _logger.LogWarning("No homestay record was updated for: {HomestayId}", homestay.HomestayId);

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating homestay: {HomestayId}", homestay.HomestayId);
                return false;
            }
        }



        public async Task<bool> DeleteHomestayAsync(int homestayId, ClaimsPrincipal user)
        {
            try
            {
                var roleClaim = user.FindFirst(ClaimTypes.Role);
                var committeeClaim = user.FindFirst("CommitteeId");

                int roleId = roleClaim != null ? int.Parse(roleClaim.Value) : 0;
                int? userCommitteeId = committeeClaim != null ? int.Parse(committeeClaim.Value) : null;

                // Fetch existing homestay details
                _dataManager.ClearParameters();
                _dataManager.AddParameter("@p_homestay_id", homestayId);
                DataTable dt = await _dataManager.ExecuteQueryAsync("GetHomestayById", CommandType.StoredProcedure);

                if (dt.Rows.Count == 0)
                {
                    _logger.LogWarning("Delete attempt failed: Homestay not found (ID: {HomestayId})", homestayId);
                    return false;
                }

                int existingCommitteeId = Convert.ToInt32(dt.Rows[0]["committee_id"]);
                if (roleId == 2 && existingCommitteeId != userCommitteeId)
                {
                    _logger.LogWarning("Unauthorized attempt to delete homestay: {HomestayId} by CommitteeId: {CommitteeId}", homestayId, userCommitteeId);
                    return false;
                }

                // Soft delete: Update is_active = 0 instead of actual deletion
                _dataManager.ClearParameters();
                _dataManager.AddParameter("@p_homestay_id", homestayId);
                _dataManager.AddParameter("@p_is_active", false);
                bool success = await _dataManager.ExecuteNonQueryAsync("ArchiveHomestay", CommandType.StoredProcedure);

                if (success)
                    _logger.LogInformation("Homestay archived successfully: {HomestayId}", homestayId);
                else
                    _logger.LogWarning("Failed to archive homestay: {HomestayId}", homestayId);

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting homestay: {HomestayId}", homestayId);
                return false;
            }
        }



        public async Task<(bool success, string message)> ArchiveHomestayAsync(int homestayId, ClaimsPrincipal user)
        {
            return await ToggleHomestayStatusAsync(homestayId, false, user);
        }

        public async Task<(bool success, string message)> UnarchiveHomestayAsync(int homestayId, ClaimsPrincipal user)
        {
            return await ToggleHomestayStatusAsync(homestayId, true, user);
        }

        private async Task<(bool success, string message)> ToggleHomestayStatusAsync(int homestayId, bool isActive, ClaimsPrincipal user)
        {
            try
            {
                var roleClaim = user.FindFirst(ClaimTypes.Role);
                var committeeClaim = user.FindFirst("CommitteeId");

                int roleId = roleClaim != null ? int.Parse(roleClaim.Value) : 0;
                int? userCommitteeId = committeeClaim != null ? int.Parse(committeeClaim.Value) : null;

                // Fetch existing homestay details
                _dataManager.ClearParameters();
                _dataManager.AddParameter("@p_homestay_id", homestayId);
                DataTable dt = await _dataManager.ExecuteQueryAsync("GetHomestayById", CommandType.StoredProcedure);

                if (dt.Rows.Count == 0)
                {
                    _logger.LogWarning("Toggle status failed: Homestay not found (ID: {HomestayId})", homestayId);
                    return (false, "Homestay does not exist.");
                }

                int existingCommitteeId = Convert.ToInt32(dt.Rows[0]["committee_id"]);
                if (roleId == 2 && existingCommitteeId != userCommitteeId)
                {
                    _logger.LogWarning("Unauthorized attempt to modify homestay status: {HomestayId} by CommitteeId: {CommitteeId}", homestayId, userCommitteeId);
                    return (false, "You are not authorized to modify this homestay.");
                }

                // Toggle homestay active status
                _dataManager.ClearParameters();
                _dataManager.AddParameter("@p_homestay_id", homestayId);
                _dataManager.AddParameter("@p_is_active", isActive);

                bool success = await _dataManager.ExecuteNonQueryAsync("ToggleHomestayStatus", CommandType.StoredProcedure);
                if (success)
                {
                    string statusMessage = isActive ? "unarchived" : "archived";
                    _logger.LogInformation("Homestay {Status} successfully: {HomestayId}", statusMessage, homestayId);
                    return (true, $"Homestay {statusMessage} successfully.");
                }
                else
                {
                    _logger.LogWarning("Failed to update homestay status: {HomestayId}", homestayId);
                    return (false, "Failed to update homestay status.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating homestay status: {HomestayId}", homestayId);
                return (false, "An error occurred while updating the homestay status.");
            }
        }


    }
}
