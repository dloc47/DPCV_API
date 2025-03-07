using DPCV_API.Configuration.DbContext;
using DPCV_API.Helpers;
using DPCV_API.Models.ActivityModel;
using System.Data;
using System.Security.Claims;
using System.Text.Json;

namespace DPCV_API.BAL.Services.Activities
{
    public class ActivityService : IActivityService
    {
        private readonly DataManager _dataManager;
        private readonly ILogger<ActivityService> _logger;

        public ActivityService(DataManager dataManager, ILogger<ActivityService> logger)
        {
            _dataManager = dataManager;
            _logger = logger;
        }

        // ✅ Get All Activities
        public async Task<List<ActivityResponseDTO>> GetAllActivitiesAsync()
        {
            string procedureName = "GetAllActivities";
            DataTable result = await _dataManager.ExecuteQueryAsync(procedureName, CommandType.StoredProcedure);
            List<ActivityResponseDTO> activities = new();

            foreach (DataRow row in result.Rows)
            {
                activities.Add(new ActivityResponseDTO
                {
                    ActivityId = Convert.ToInt32(row["activity_id"]),
                    ActivityName = row["activity_name"].ToString()!,
                    Description = row["description"]?.ToString(),
                    Tags = JsonHelper.DeserializeJsonSafely<List<string>>(row["tags"], "tags"),
                    CommitteeId = Convert.ToInt32(row["committee_id"]),
                    HomestayId = row["homestay_id"] != DBNull.Value ? Convert.ToInt32(row["homestay_id"]) : null,
                    IsVerifiable = Convert.ToBoolean(row["isVerifiable"]),
                    VerificationStatusId = row["verification_status_id"] != DBNull.Value ? Convert.ToInt32(row["verification_status_id"]) : null,
                    is_active = Convert.ToInt32(row["is_active"]),
                    DistrictId = Convert.ToInt32(row["district_id"]),
                    DistrictName = row["district_name"].ToString()!,
                    Region = row["region"].ToString()!,
                    StatusType = row["status_type"] != DBNull.Value ? row["status_type"].ToString()! : null
                });
            }
            return activities;
        }



        // ✅ Get Activity by ID
        public async Task<ActivityResponseDTO?> GetActivityByIdAsync(int activityId)
        {
            string procedureName = "GetActivityById";
            _dataManager.ClearParameters();
            _dataManager.AddParameter("@p_activity_id", activityId);

            DataTable result = await _dataManager.ExecuteQueryAsync(procedureName, CommandType.StoredProcedure);
            if (result.Rows.Count == 0) return null;

            DataRow row = result.Rows[0];
            return new ActivityResponseDTO
            {
                ActivityId = Convert.ToInt32(row["activity_id"]),
                ActivityName = row["activity_name"].ToString()!,
                Description = row["description"]?.ToString(),
                Tags = JsonHelper.DeserializeJsonSafely<List<string>>(row["tags"], "tags"),
                CommitteeId = Convert.ToInt32(row["committee_id"]),
                HomestayId = row["homestay_id"] != DBNull.Value ? Convert.ToInt32(row["homestay_id"]) : null,
                IsVerifiable = Convert.ToBoolean(row["isVerifiable"]),
                VerificationStatusId = row["verification_status_id"] != DBNull.Value ? Convert.ToInt32(row["verification_status_id"]) : null,
                is_active = Convert.ToInt32(row["is_active"]),
                DistrictId = Convert.ToInt32(row["district_id"]),
                DistrictName = row["district_name"].ToString()!,
                Region = row["region"].ToString()!,
                StatusType = row["status_type"] != DBNull.Value ? row["status_type"].ToString()! : null
            };
        }


        // ✅ Create Activity with Role-Based Validation
        public async Task<bool> CreateActivityAsync(ActivityDTO activity, ClaimsPrincipal user)
        {
            try
            {
                var roleClaim = user.FindFirst(ClaimTypes.Role);
                var committeeClaim = user.FindFirst("CommitteeId");

                int roleId = roleClaim != null ? int.Parse(roleClaim.Value) : 0;
                int? committeeId = committeeClaim != null ? int.Parse(committeeClaim.Value) : null;

                if (roleId == 0 || roleId == 2 && committeeId != activity.CommitteeId)
                {
                    _logger.LogWarning("Unauthorized attempt to create an activity.");
                    return false;
                }

                int verificationStatus = roleId == 1 ? 2 : 1;

                string procedureName = "CreateActivity";
                _dataManager.ClearParameters();
                _dataManager.AddParameter("@p_activity_name", activity.ActivityName);
                _dataManager.AddParameter("@p_description", string.IsNullOrWhiteSpace(activity.Description) ? DBNull.Value : activity.Description);
                _dataManager.AddParameter("@p_tags", JsonSerializer.Serialize(activity.Tags));
                _dataManager.AddParameter("@p_committee_id", activity.CommitteeId);
                _dataManager.AddParameter("@p_homestay_id", activity.HomestayId.HasValue ? activity.HomestayId.Value : DBNull.Value);
                _dataManager.AddParameter("@p_isVerifiable", activity.IsVerifiable);
                _dataManager.AddParameter("@p_verification_status_id", verificationStatus);
                _dataManager.AddParameter("@p_is_active", activity.is_active);

                bool success = await _dataManager.ExecuteNonQueryAsync(procedureName, CommandType.StoredProcedure);
                if (success)
                {
                    _logger.LogInformation("Activity created successfully: {ActivityName}", activity.ActivityName);
                }
                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating activity: {ActivityName}", activity.ActivityName);
                return false;
            }
        }

        // ✅ Update Activity with Role-Based Validation
        public async Task<bool> UpdateActivityAsync(int activityId, ActivityDTO activity, ClaimsPrincipal user)
        {
            try
            {
                var roleClaim = user.FindFirst(ClaimTypes.Role);
                var committeeClaim = user.FindFirst("CommitteeId");

                int roleId = roleClaim != null ? int.Parse(roleClaim.Value) : 0;
                int? committeeId = committeeClaim != null ? int.Parse(committeeClaim.Value) : null;

                _dataManager.ClearParameters();
                _dataManager.AddParameter("p_activity_id", activityId);
                DataTable dt = await _dataManager.ExecuteQueryAsync("GetActivityById", CommandType.StoredProcedure);
                if (dt.Rows.Count == 0) return false;

                int existingCommitteeId = Convert.ToInt32(dt.Rows[0]["committee_id"]);
                if (roleId == 2 && existingCommitteeId != committeeId)
                {
                    _logger.LogWarning("Unauthorized attempt to update activity: {ActivityId}", activityId);
                    return false;
                }

                int verificationStatus = roleId == 1 ? 2 : 1;

                string procedureName = "UpdateActivity";
                _dataManager.ClearParameters();
                _dataManager.AddParameter("@p_activity_id", activityId);
                _dataManager.AddParameter("@p_activity_name", string.IsNullOrWhiteSpace(activity.ActivityName) ? DBNull.Value : activity.ActivityName);
                _dataManager.AddParameter("@p_description", string.IsNullOrWhiteSpace(activity.Description) ? DBNull.Value : activity.Description);
                _dataManager.AddParameter("@p_tags", JsonSerializer.Serialize(activity.Tags));
                _dataManager.AddParameter("@p_committee_id", activity.CommitteeId);
                _dataManager.AddParameter("@p_homestay_id", activity.HomestayId ?? (object)DBNull.Value);
                _dataManager.AddParameter("@p_isVerifiable", activity.IsVerifiable);
                _dataManager.AddParameter("@p_verification_status_id", verificationStatus);
                _dataManager.AddParameter("@p_is_active", activity.is_active);

                DataTable result = await _dataManager.ExecuteQueryAsync(procedureName, CommandType.StoredProcedure);
                bool success = result.Rows.Count > 0 && Convert.ToInt32(result.Rows[0]["success"]) == 1;

                if (success)
                    _logger.LogInformation("Activity updated successfully: {ActivityId}", activityId);
                else
                    _logger.LogWarning("No activity record was updated for: {ActivityId}", activityId);

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating activity: {ActivityId}", activityId);
                return false;
            }
        }


        // ✅ Delete Activity with Role-Based Validation
        public async Task<bool> DeleteActivityAsync(int activityId, ClaimsPrincipal user)
        {
            try
            {
                var roleClaim = user.FindFirst(ClaimTypes.Role);
                var committeeClaim = user.FindFirst("CommitteeId");

                int roleId = roleClaim != null ? int.Parse(roleClaim.Value) : 0;
                int? committeeId = committeeClaim != null ? int.Parse(committeeClaim.Value) : null;

                _dataManager.ClearParameters();
                _dataManager.AddParameter("p_activity_id", activityId);
                DataTable dt = await _dataManager.ExecuteQueryAsync("GetActivityById", CommandType.StoredProcedure);
                if (dt.Rows.Count == 0) return false;

                int existingCommitteeId = Convert.ToInt32(dt.Rows[0]["committee_id"]);
                if (roleId == 2 && existingCommitteeId != committeeId)
                {
                    _logger.LogWarning("Unauthorized attempt to delete activity: {ActivityId}", activityId);
                    return false;
                }

                _dataManager.ClearParameters();
                _dataManager.AddParameter("p_activity_id", activityId);
                DataTable result = await _dataManager.ExecuteQueryAsync("DeleteActivity", CommandType.StoredProcedure);
                bool success = result.Rows.Count > 0 && Convert.ToInt32(result.Rows[0]["success"]) == 1;

                if (success)
                    _logger.LogInformation("Activity deleted successfully: {ActivityId}", activityId);
                else
                    _logger.LogWarning("No activity record was deleted for: {ActivityId}", activityId);

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting activity: {ActivityId}", activityId);
                return false;
            }
        }

        // ✅ Archive Activity (Set is_active = 0)
        public async Task<(bool success, string message)> ArchiveActivityAsync(int activityId, ClaimsPrincipal user)
        {
            return await ToggleActivityStatusAsync(activityId, false, user);
        }

        // ✅ Unarchive Activity (Set is_active = 1)
        public async Task<(bool success, string message)> UnarchiveActivityAsync(int activityId, ClaimsPrincipal user)
        {
            return await ToggleActivityStatusAsync(activityId, true, user);
        }

        // ✅ Helper Method to Toggle is_active
        // ✅ Helper Method to Toggle is_active
        private async Task<(bool success, string message)> ToggleActivityStatusAsync(int activityId, bool isActive, ClaimsPrincipal user)
        {
            try
            {
                var roleClaim = user.FindFirst(ClaimTypes.Role);
                var committeeClaim = user.FindFirst("CommitteeId");

                int roleId = roleClaim != null ? int.Parse(roleClaim.Value) : 0;
                int? committeeId = committeeClaim != null ? int.Parse(committeeClaim.Value) : null;

                // Check if the activity exists and get current is_active status
                _dataManager.ClearParameters();
                _dataManager.AddParameter("p_activity_id", activityId);
                DataTable dt = await _dataManager.ExecuteQueryAsync("GetActivityById", CommandType.StoredProcedure);
                if (dt.Rows.Count == 0)
                    return (false, "Activity does not exist.");

                int existingCommitteeId = Convert.ToInt32(dt.Rows[0]["committee_id"]);
                bool currentStatus = Convert.ToBoolean(dt.Rows[0]["is_active"]);

                // If the status is already set, return an appropriate message
                if (currentStatus == isActive)
                {
                    string alreadyMessage = isActive ? "Activity is already active." : "Activity is already archived.";
                    return (false, alreadyMessage);
                }

                if (roleId == 2 && existingCommitteeId != committeeId)
                {
                    _logger.LogWarning("Unauthorized attempt to modify activity status: {ActivityId}", activityId);
                    return (false, "You are not authorized to modify this activity.");
                }

                // Call stored procedure to update is_active
                _dataManager.ClearParameters();
                _dataManager.AddParameter("@p_activity_id", activityId);
                _dataManager.AddParameter("@p_is_active", isActive);

                DataTable result = await _dataManager.ExecuteQueryAsync("ToggleActivityStatus", CommandType.StoredProcedure);
                bool success = result.Rows.Count > 0 && Convert.ToInt32(result.Rows[0]["success"]) == 1;

                if (success)
                {
                    _logger.LogInformation("Activity status updated successfully: {ActivityId}, Active: {IsActive}", activityId, isActive);
                    return (true, isActive ? "Activity unarchived successfully." : "Activity archived successfully.");
                }
                else
                {
                    _logger.LogWarning("No activity record was updated for: {ActivityId}", activityId);
                    return (false, "Failed to update activity status.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating activity status: {ActivityId}", activityId);
                return (false, "An error occurred while updating the activity status.");
            }
        }



    }
}
