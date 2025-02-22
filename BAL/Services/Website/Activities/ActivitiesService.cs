using DPCV_API.BAL.Services.Website.Activities;
using DPCV_API.Configuration.DbContext;
using DPCV_API.Models.ActivityModel;
using System.Data;

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

        // ✅ Create Activity
        public async Task<bool> CreateActivityAsync(ActivityDTO activity)
        {
            try
            {
                string procedureName = "CreateActivity";
                _dataManager.ClearParameters();
                _dataManager.AddParameter("@p_activity_name", activity.ActivityName);
                _dataManager.AddParameter("@p_description", string.IsNullOrWhiteSpace(activity.Description) ? DBNull.Value : activity.Description);
                _dataManager.AddParameter("@p_committee_id", activity.CommitteeId);
                _dataManager.AddParameter("@p_homestay_id", activity.HomestayId.HasValue ? activity.HomestayId.Value : DBNull.Value);
                _dataManager.AddParameter("@p_isVerifiable", activity.IsVerifiable);
                _dataManager.AddParameter("@p_verification_status_id", activity.VerificationStatusId.HasValue ? activity.VerificationStatusId.Value : DBNull.Value);

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

        // ✅ Update Activity
        public async Task<bool> UpdateActivityAsync(int activityId, ActivityDTO activity)
        {
            try
            {
                string procedureName = "UpdateActivity";
                _dataManager.ClearParameters();

                _dataManager.AddParameter("@p_activity_id", activityId);
                _dataManager.AddParameter("@p_activity_name", string.IsNullOrWhiteSpace(activity.ActivityName) ? DBNull.Value : activity.ActivityName);
                _dataManager.AddParameter("@p_description", string.IsNullOrWhiteSpace(activity.Description) ? DBNull.Value : activity.Description);
                _dataManager.AddParameter("@p_committee_id", activity.CommitteeId);
                _dataManager.AddParameter("@p_homestay_id", activity.HomestayId.HasValue ? activity.HomestayId.Value : DBNull.Value);
                _dataManager.AddParameter("@p_isVerifiable", activity.IsVerifiable);
                _dataManager.AddParameter("@p_verification_status_id", activity.VerificationStatusId.HasValue ? activity.VerificationStatusId.Value : DBNull.Value);

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

        // ✅ Delete Activity
        public async Task<bool> DeleteActivityAsync(int activityId)
        {
            try
            {
                string query = "DELETE FROM activities WHERE activity_id = @ActivityId";
                _dataManager.ClearParameters();
                _dataManager.AddParameter("@ActivityId", activityId);

                bool success = await _dataManager.ExecuteNonQueryAsync(query, CommandType.Text);
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
