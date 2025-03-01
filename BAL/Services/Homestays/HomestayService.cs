using System.Security.Claims;
using DPCV_API.Configuration.DbContext;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using DPCV_API.Models.HomestayModel;

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

        public async Task<List<HomestayDTO>> GetAllHomestaysAsync()
        {
            string spName = "GetAllHomestays";
            List<HomestayDTO> homestays = new();

            try
            {
                DataTable result = await _dataManager.ExecuteQueryAsync(spName, CommandType.StoredProcedure);

                foreach (DataRow row in result.Rows)
                {
                    homestays.Add(new HomestayDTO
                    {
                        HomestayId = Convert.ToInt32(row["homestay_id"]),
                        HomestayName = row["homestay_name"].ToString()!,
                        CommitteeId = Convert.ToInt32(row["committee_id"]),
                        Address = row["address"].ToString()!,
                        OwnerName = row["owner_name"].ToString()!,
                        OwnerMobile = row["owner_mobile"].ToString()!,
                        TotalRooms = Convert.ToInt32(row["total_rooms"]),
                        RoomTariff = Convert.ToDecimal(row["room_tariff"]),
                        Tags = row["tags"] != DBNull.Value ? JsonDocument.Parse(row["tags"].ToString()!) : null,
                        IsVerifiable = Convert.ToBoolean(row["isVerifiable"]),
                        VerificationStatusId = row["verification_status_id"] != DBNull.Value ? Convert.ToInt32(row["verification_status_id"]) : null,
                        is_active = Convert.ToInt32(row["is_active"])
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

        public async Task<HomestayDTO?> GetHomestayByIdAsync(int homestayId)
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
                return new HomestayDTO
                {
                    HomestayId = Convert.ToInt32(row["homestay_id"]),
                    HomestayName = row["homestay_name"].ToString()!,
                    CommitteeId = Convert.ToInt32(row["committee_id"]),
                    Address = row["address"].ToString()!,
                    OwnerName = row["owner_name"].ToString()!,
                    OwnerMobile = row["owner_mobile"].ToString()!,
                    TotalRooms = Convert.ToInt32(row["total_rooms"]),
                    RoomTariff = Convert.ToDecimal(row["room_tariff"]),
                    Tags = row["tags"] != DBNull.Value ? JsonDocument.Parse(row["tags"].ToString()!) : null,
                    IsVerifiable = Convert.ToBoolean(row["isVerifiable"]),
                    VerificationStatusId = row["verification_status_id"] != DBNull.Value ? Convert.ToInt32(row["verification_status_id"]) : null,
                    is_active = Convert.ToInt32(row["is_active"])
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
                _dataManager.AddParameter("@p_owner_name", homestay.OwnerName);
                _dataManager.AddParameter("@p_owner_mobile", homestay.OwnerMobile);
                _dataManager.AddParameter("@p_total_rooms", homestay.TotalRooms);
                _dataManager.AddParameter("@p_room_tariff", homestay.RoomTariff);
                _dataManager.AddParameter("@p_tags", homestay.Tags?.RootElement.ToString() ?? (object)DBNull.Value);
                _dataManager.AddParameter("@p_isVerifiable", homestay.IsVerifiable);
                _dataManager.AddParameter("@p_verification_status_id", verificationStatus);
                _dataManager.AddParameter("@p_is_active", homestay.is_active);

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
                _dataManager.AddParameter("p_homestay_id", homestay.HomestayId);
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
                _dataManager.AddParameter("@p_owner_name", homestay.OwnerName);
                _dataManager.AddParameter("@p_owner_mobile", homestay.OwnerMobile);
                _dataManager.AddParameter("@p_total_rooms", homestay.TotalRooms);
                _dataManager.AddParameter("@p_room_tariff", homestay.RoomTariff);
                _dataManager.AddParameter("@p_tags", homestay.Tags?.RootElement.ToString() ?? (object)DBNull.Value);
                _dataManager.AddParameter("@p_isVerifiable", homestay.IsVerifiable);
                _dataManager.AddParameter("@p_verification_status_id", verificationStatus);
                _dataManager.AddParameter("@p_is_active", homestay.is_active);

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
                _dataManager.AddParameter("p_homestay_id", homestayId);
                DataTable dt = await _dataManager.ExecuteQueryAsync("GetHomestayById", CommandType.StoredProcedure);

                if (dt.Rows.Count == 0)
                    return false;

                int existingCommitteeId = Convert.ToInt32(dt.Rows[0]["committee_id"]);
                if (roleId == 2 && existingCommitteeId != userCommitteeId)
                {
                    _logger.LogWarning("Unauthorized attempt to delete homestay: {HomestayId}", homestayId);
                    return false;
                }

                _dataManager.ClearParameters();
                _dataManager.AddParameter("@p_homestay_id", homestayId);
                bool success = await _dataManager.ExecuteNonQueryAsync("DeleteHomestay", CommandType.StoredProcedure);

                if (success)
                    _logger.LogInformation("Homestay deleted successfully: {HomestayId}", homestayId);
                else
                    _logger.LogWarning("No homestay record was deleted for: {HomestayId}", homestayId);

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

                _dataManager.ClearParameters();
                _dataManager.AddParameter("p_homestay_id", homestayId);
                DataTable dt = await _dataManager.ExecuteQueryAsync("GetHomestayById", CommandType.StoredProcedure);
                if (dt.Rows.Count == 0)
                    return (false, "Homestay does not exist.");

                if (roleId == 2 && Convert.ToInt32(dt.Rows[0]["committee_id"]) != userCommitteeId)
                    return (false, "You are not authorized to modify this homestay.");

                _dataManager.ClearParameters();
                _dataManager.AddParameter("@p_homestay_id", homestayId);
                _dataManager.AddParameter("@p_is_active", isActive);

                return await _dataManager.ExecuteNonQueryAsync("ToggleHomestayStatus", CommandType.StoredProcedure)
                    ? (true, isActive ? "Homestay unarchived successfully." : "Homestay archived successfully.")
                    : (false, "Failed to update homestay status.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating homestay status.");
                return (false, "An error occurred while updating the homestay status.");
            }
        }
    }
}
