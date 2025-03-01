using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using DPCV_API.Configuration.DbContext;
using DPCV_API.Models.Website.EventModel;
using Microsoft.Extensions.Logging;

namespace DPCV_API.BAL.Services.Events
{
    public class EventService : IEventService
    {
        private readonly DataManager _dataManager;
        private readonly ILogger<EventService> _logger;

        public EventService(DataManager dataManager, ILogger<EventService> logger)
        {
            _dataManager = dataManager;
            _logger = logger;
        }

        public async Task<IEnumerable<EventDTO>> GetAllEventsAsync()
        {
            string spName = "GetAllEvents";
            List<EventDTO> events = new();

            try
            {
                DataTable result = await _dataManager.ExecuteQueryAsync(spName, CommandType.StoredProcedure);
                foreach (DataRow row in result.Rows)
                {
                    events.Add(new EventDTO
                    {
                        EventId = Convert.ToInt32(row["event_id"]),
                        EventName = row["event_name"].ToString()!,
                        Description = row["description"].ToString()!,
                        StartDate = Convert.ToDateTime(row["start_date"]),
                        EndDate = Convert.ToDateTime(row["end_date"]),
                        Location = row["location"].ToString()!,
                        CommitteeId = Convert.ToInt32(row["committee_id"]),
                        Tags = row["tags"] != DBNull.Value ? JsonDocument.Parse(row["tags"].ToString()!) : null,
                        IsVerifiable = Convert.ToBoolean(row["isVerifiable"]),
                        VerificationStatusId = row["verification_status_id"] != DBNull.Value ? Convert.ToInt32(row["verification_status_id"]) : null,
                        is_active = Convert.ToInt32(row["is_active"])
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching events.");
                throw;
            }
            return events;
        }

        public async Task<EventDTO?> GetEventByIdAsync(int eventId)
        {
            string spName = "GetEventById";
            _dataManager.ClearParameters();
            _dataManager.AddParameter("@p_event_id", eventId);

            try
            {
                DataTable result = await _dataManager.ExecuteQueryAsync(spName, CommandType.StoredProcedure);
                if (result.Rows.Count == 0) return null;

                DataRow row = result.Rows[0];
                return new EventDTO
                {
                    EventId = Convert.ToInt32(row["event_id"]),
                    EventName = row["event_name"].ToString()!,
                    Description = row["description"].ToString()!,
                    StartDate = Convert.ToDateTime(row["start_date"]),
                    EndDate = Convert.ToDateTime(row["end_date"]),
                    Location = row["location"].ToString()!,
                    CommitteeId = Convert.ToInt32(row["committee_id"]),
                    Tags = row["tags"] != DBNull.Value ? JsonDocument.Parse(row["tags"].ToString()!) : null,
                    IsVerifiable = Convert.ToBoolean(row["isVerifiable"]),
                    VerificationStatusId = row["verification_status_id"] != DBNull.Value ? Convert.ToInt32(row["verification_status_id"]) : null,
                    is_active = Convert.ToInt32(row["is_active"])
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching event with ID {EventId}", eventId);
                throw;
            }
        }

        public async Task<bool> CreateEventAsync(EventDTO eventDto, ClaimsPrincipal user)
        {
            var roleClaim = user.FindFirst(ClaimTypes.Role);
            var committeeClaim = user.FindFirst("CommitteeId");

            int roleId = roleClaim != null ? int.Parse(roleClaim.Value) : 0;
            int? userCommitteeId = committeeClaim != null ? int.Parse(committeeClaim.Value) : null;

            if (roleId == 2 && userCommitteeId != eventDto.CommitteeId)
            {
                _logger.LogWarning("Unauthorized attempt to create an event.");
                return false;
            }

            string procedureName = "CreateEvent";
            _dataManager.ClearParameters();
            _dataManager.AddParameter("@p_event_name", eventDto.EventName);
            _dataManager.AddParameter("@p_description", eventDto.Description);
            _dataManager.AddParameter("@p_start_date", eventDto.StartDate);
            _dataManager.AddParameter("@p_end_date", eventDto.EndDate);
            _dataManager.AddParameter("@p_location", eventDto.Location);
            _dataManager.AddParameter("@p_committee_id", eventDto.CommitteeId);
            _dataManager.AddParameter("@p_tags", eventDto.Tags?.RootElement.ToString() ?? (object)DBNull.Value);
            _dataManager.AddParameter("@p_isVerifiable", eventDto.IsVerifiable);
            _dataManager.AddParameter("@p_verification_status_id", roleId == 1 ? 2 : 1);
            _dataManager.AddParameter("@p_is_active", eventDto.is_active);

            try
            {
                bool success = await _dataManager.ExecuteNonQueryAsync(procedureName, CommandType.StoredProcedure);
                if (success)
                {
                    _logger.LogInformation("Event created successfully: {EventName}", eventDto.EventName);
                }
                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating event: {EventName}", eventDto.EventName);
                return false;
            }
        }

        public async Task<bool> UpdateEventAsync(EventDTO eventDto, ClaimsPrincipal user)
        {
            var roleClaim = user.FindFirst(ClaimTypes.Role);
            var committeeClaim = user.FindFirst("CommitteeId");

            int roleId = roleClaim != null ? int.Parse(roleClaim.Value) : 0;
            int? userCommitteeId = committeeClaim != null ? int.Parse(committeeClaim.Value) : null;

            if (roleId == 2 && userCommitteeId != eventDto.CommitteeId)
            {
                _logger.LogWarning("Unauthorized attempt to update event: {EventId}", eventDto.EventId);
                return false;
            }

            string procedureName = "UpdateEvent";
            _dataManager.ClearParameters();
            _dataManager.AddParameter("@p_event_id", eventDto.EventId);
            _dataManager.AddParameter("@p_event_name", eventDto.EventName);
            _dataManager.AddParameter("@p_description", eventDto.Description);
            _dataManager.AddParameter("@p_start_date", eventDto.StartDate);
            _dataManager.AddParameter("@p_end_date", eventDto.EndDate);
            _dataManager.AddParameter("@p_location", eventDto.Location);
            _dataManager.AddParameter("@p_committee_id", eventDto.CommitteeId);
            _dataManager.AddParameter("@p_tags", eventDto.Tags?.RootElement.ToString() ?? (object)DBNull.Value);
            _dataManager.AddParameter("@p_isVerifiable", eventDto.IsVerifiable);
            _dataManager.AddParameter("@p_verification_status_id", roleId == 1 ? 2 : 1);
            _dataManager.AddParameter("@p_is_active", eventDto.is_active);

            try
            {
                bool success = await _dataManager.ExecuteNonQueryAsync(procedureName, CommandType.StoredProcedure);
                if (success)
                {
                    _logger.LogInformation("Event updated successfully: {EventId}", eventDto.EventId);
                }
                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating event: {EventId}", eventDto.EventId);
                return false;
            }
        }

        public async Task<bool> DeleteEventAsync(int eventId, ClaimsPrincipal user)
        {
            var roleClaim = user.FindFirst(ClaimTypes.Role);
            var committeeClaim = user.FindFirst("CommitteeId");

            int roleId = roleClaim != null ? int.Parse(roleClaim.Value) : 0;
            int? userCommitteeId = committeeClaim != null ? int.Parse(committeeClaim.Value) : null;

            string checkProcedure = "GetEventById";
            _dataManager.ClearParameters();
            _dataManager.AddParameter("@p_event_id", eventId);

            DataTable result;
            try
            {
                result = await _dataManager.ExecuteQueryAsync(checkProcedure, CommandType.StoredProcedure);
                if (result.Rows.Count == 0)
                {
                    _logger.LogWarning("Event not found: {EventId}", eventId);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching event for deletion: {EventId}", eventId);
                return false;
            }

            DataRow row = result.Rows[0];
            int committeeId = Convert.ToInt32(row["committee_id"]);

            if (roleId == 2 && userCommitteeId != committeeId)
            {
                _logger.LogWarning("Unauthorized attempt to delete event: {EventId}", eventId);
                return false;
            }

            string procedureName = "DeleteEvent";
            _dataManager.ClearParameters();
            _dataManager.AddParameter("@p_event_id", eventId);

            try
            {
                bool success = await _dataManager.ExecuteNonQueryAsync(procedureName, CommandType.StoredProcedure);
                if (success)
                {
                    _logger.LogInformation("Event deleted successfully: {EventId}", eventId);
                }
                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting event: {EventId}", eventId);
                return false;
            }
        }

        // ✅ Archive Event (Set is_active = 0)
        public async Task<(bool success, string message)> ArchiveEventAsync(int eventId, ClaimsPrincipal user)
        {
            return await ToggleEventStatusAsync(eventId, false, user);
        }

        // ✅ Unarchive Event (Set is_active = 1)
        public async Task<(bool success, string message)> UnarchiveEventAsync(int eventId, ClaimsPrincipal user)
        {
            return await ToggleEventStatusAsync(eventId, true, user);
        }

        // ✅ Helper Method to Toggle is_active
        private async Task<(bool success, string message)> ToggleEventStatusAsync(int eventId, bool isActive, ClaimsPrincipal user)
        {
            try
            {
                var roleClaim = user.FindFirst(ClaimTypes.Role);
                var committeeClaim = user.FindFirst("CommitteeId");

                int roleId = roleClaim != null ? int.Parse(roleClaim.Value) : 0;
                int? userCommitteeId = committeeClaim != null ? int.Parse(committeeClaim.Value) : null;

                // Check if the event exists and get current is_active status
                _dataManager.ClearParameters();
                _dataManager.AddParameter("@p_event_id", eventId);
                DataTable dt = await _dataManager.ExecuteQueryAsync("GetEventById", CommandType.StoredProcedure);
                if (dt.Rows.Count == 0)
                    return (false, "Event does not exist.");

                int existingCommitteeId = Convert.ToInt32(dt.Rows[0]["committee_id"]);
                bool currentStatus = Convert.ToBoolean(dt.Rows[0]["is_active"]);

                // If the status is already set, return an appropriate message
                if (currentStatus == isActive)
                {
                    string alreadyMessage = isActive ? "Event is already active." : "Event is already archived.";
                    return (false, alreadyMessage);
                }

                if (roleId == 2 && existingCommitteeId != userCommitteeId)
                {
                    _logger.LogWarning("Unauthorized attempt to modify event status: {EventId}", eventId);
                    return (false, "You are not authorized to modify this event.");
                }

                // Call stored procedure to update is_active
                _dataManager.ClearParameters();
                _dataManager.AddParameter("@p_event_id", eventId);
                _dataManager.AddParameter("@p_is_active", isActive);

                DataTable result = await _dataManager.ExecuteQueryAsync("ToggleEventStatus", CommandType.StoredProcedure);
                bool success = result.Rows.Count > 0 && Convert.ToInt32(result.Rows[0]["success"]) == 1;

                if (success)
                {
                    _logger.LogInformation("Event status updated successfully: {EventId}, Active: {IsActive}", eventId, isActive);
                    return (true, isActive ? "Event unarchived successfully." : "Event archived successfully.");
                }
                else
                {
                    _logger.LogWarning("No event record was updated for: {EventId}", eventId);
                    return (false, "Failed to update event status.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating event status: {EventId}", eventId);
                return (false, "An error occurred while updating the event status.");
            }
        }

    }
}
