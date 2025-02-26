using DPCV_API.Configuration.DbContext;
using DPCV_API.Models.ActivityModel;
using System.Data;
using System.Security.Claims;

namespace DPCV_API.BAL.Services.Website.Activities
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
        public async Task<List<ActivityDTO>> GetAllActivitiesAsync()
        {
            string query = "SELECT * FROM activities";
            DataTable result = await _dataManager.ExecuteQueryAsync(query, CommandType.Text);
            List<ActivityDTO> activities = new();

            foreach (DataRow row in result.Rows)
            {
                activities.Add(new ActivityDTO
                {
                    ActivityId = Convert.ToInt32(row["activity_id"]),
                    ActivityName = row["activity_name"].ToString()!,
                    Description = row["description"]?.ToString(),
                    CommitteeId = Convert.ToInt32(row["committee_id"]),
                    HomestayId = row["homestay_id"] != DBNull.Value ? Convert.ToInt32(row["homestay_id"]) : null,
                    IsVerifiable = Convert.ToBoolean(row["isVerifiable"]),
                    VerificationStatusId = row["verification_status_id"] != DBNull.Value ? Convert.ToInt32(row["verification_status_id"]) : null
                });
            }
            return activities;
        }

        // ✅ Get Activity by ID
        public async Task<ActivityDTO?> GetActivityByIdAsync(int activityId)
        {
            string query = "SELECT * FROM activities WHERE activity_id = @ActivityId";
            _dataManager.ClearParameters();
            _dataManager.AddParameter("@ActivityId", activityId);

            DataTable result = await _dataManager.ExecuteQueryAsync(query, CommandType.Text);
            if (result.Rows.Count == 0) return null;

            DataRow row = result.Rows[0];
            return new ActivityDTO
            {
                ActivityId = Convert.ToInt32(row["activity_id"]),
                ActivityName = row["activity_name"].ToString()!,
                Description = row["description"]?.ToString(),
                CommitteeId = Convert.ToInt32(row["committee_id"]),
                HomestayId = row["homestay_id"] != DBNull.Value ? Convert.ToInt32(row["homestay_id"]) : null,
                IsVerifiable = Convert.ToBoolean(row["isVerifiable"]),
                VerificationStatusId = row["verification_status_id"] != DBNull.Value ? Convert.ToInt32(row["verification_status_id"]) : null
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
                int? committeeId = committeeClaim != null ? int.Parse(committeeClaim.Value) : (int?)null;

                if (roleId == 0 || (roleId == 2 && committeeId != activity.CommitteeId))
                {
                    _logger.LogWarning("Unauthorized attempt to create an activity.");
                    return false;
                }

                int verificationStatus = (roleId == 1) ? 2 : 1;

                string procedureName = "CreateActivity";
                _dataManager.ClearParameters();
                _dataManager.AddParameter("@p_activity_name", activity.ActivityName);
                _dataManager.AddParameter("@p_description", string.IsNullOrWhiteSpace(activity.Description) ? DBNull.Value : activity.Description);
                _dataManager.AddParameter("@p_committee_id", activity.CommitteeId);
                _dataManager.AddParameter("@p_homestay_id", activity.HomestayId.HasValue ? activity.HomestayId.Value : DBNull.Value);
                _dataManager.AddParameter("@p_isVerifiable", activity.IsVerifiable);
                _dataManager.AddParameter("@p_verification_status_id", verificationStatus);

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
                int? committeeId = committeeClaim != null ? int.Parse(committeeClaim.Value) : (int?)null;

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

                int verificationStatus = (roleId == 1) ? 2 : 1;

                string procedureName = "UpdateActivity";
                _dataManager.ClearParameters();
                _dataManager.AddParameter("@p_activity_id", activityId);
                _dataManager.AddParameter("@p_activity_name", string.IsNullOrWhiteSpace(activity.ActivityName) ? DBNull.Value : activity.ActivityName);
                _dataManager.AddParameter("@p_description", string.IsNullOrWhiteSpace(activity.Description) ? DBNull.Value : activity.Description);
                _dataManager.AddParameter("@p_committee_id", activity.CommitteeId);
                _dataManager.AddParameter("@p_homestay_id", activity.HomestayId.HasValue ? activity.HomestayId.Value : DBNull.Value);
                _dataManager.AddParameter("@p_isVerifiable", activity.IsVerifiable);
                _dataManager.AddParameter("@p_verification_status_id", verificationStatus);

                bool success = await _dataManager.ExecuteNonQueryAsync(procedureName, CommandType.StoredProcedure);
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
                int? committeeId = committeeClaim != null ? int.Parse(committeeClaim.Value) : (int?)null;

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
                bool success = await _dataManager.ExecuteNonQueryAsync("DeleteActivity", CommandType.StoredProcedure);

                if (success)
                {
                    _logger.LogInformation("Activity deleted successfully: {ActivityId}", activityId);
                }
                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting activity: {ActivityId}", activityId);
                return false;
            }
        }
    }
}
