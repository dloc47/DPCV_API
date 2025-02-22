using System.Text.Json;
using System.Data;
using DPCV_API.Configuration.DbContext;
using DPCV_API.Models.Website.EventModel;

namespace DPCV_API.BAL.Services.Website.Events
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
            try
            {
                string query = "SELECT * FROM events";
                DataTable dt = await _dataManager.ExecuteQueryAsync(query, CommandType.Text);
                var events = new List<EventDTO>();
                foreach (DataRow row in dt.Rows)
                {
                    events.Add(MapEvent(row));
                }
                return events;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching events");
                return Enumerable.Empty<EventDTO>();
            }
        }

        public async Task<EventDTO?> GetEventByIdAsync(int eventId)
        {
            try
            {
                string query = "SELECT * FROM events WHERE event_id = @p_event_id";
                _dataManager.ClearParameters();
                _dataManager.AddParameter("@p_event_id", eventId);
                DataTable dt = await _dataManager.ExecuteQueryAsync(query, CommandType.Text);
                if (dt.Rows.Count == 0) return null;
                return MapEvent(dt.Rows[0]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching event with ID {EventId}", eventId);
                return null;
            }
        }

        public async Task<bool> CreateEventAsync(EventDTO eventDto)
        {
            try
            {
                string procedureName = "CreateEvent";
                _dataManager.ClearParameters();
                _dataManager.AddParameter("@p_event_name", eventDto.EventName);
                _dataManager.AddParameter("@p_description", eventDto.Description);
                _dataManager.AddParameter("@p_start_date", eventDto.StartDate);
                _dataManager.AddParameter("@p_end_date", eventDto.EndDate);
                _dataManager.AddParameter("@p_location", eventDto.Location);
                _dataManager.AddParameter("@p_committee_id", eventDto.CommitteeId);
                _dataManager.AddParameter("@p_tags", eventDto.Tags != null ? eventDto.Tags.RootElement.ToString() : DBNull.Value);
                _dataManager.AddParameter("@p_isVerifiable", eventDto.IsVerifiable);
                _dataManager.AddParameter("@p_verification_status_id", eventDto.VerificationStatusId.HasValue ? eventDto.VerificationStatusId.Value : DBNull.Value);

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

        public async Task<bool> UpdateEventAsync(int eventId, EventDTO eventDto)
        {
            try
            {
                string procedureName = "UpdateEvent";
                _dataManager.ClearParameters();
                _dataManager.AddParameter("@p_event_id", eventId);
                _dataManager.AddParameter("@p_event_name", eventDto.EventName);
                _dataManager.AddParameter("@p_description", eventDto.Description);
                _dataManager.AddParameter("@p_start_date", eventDto.StartDate);
                _dataManager.AddParameter("@p_end_date", eventDto.EndDate);
                _dataManager.AddParameter("@p_location", eventDto.Location);
                _dataManager.AddParameter("@p_committee_id", eventDto.CommitteeId);
                _dataManager.AddParameter("@p_tags", eventDto.Tags != null ? eventDto.Tags.RootElement.ToString() : DBNull.Value);
                _dataManager.AddParameter("@p_isVerifiable", eventDto.IsVerifiable);
                _dataManager.AddParameter("@p_verification_status_id", eventDto.VerificationStatusId.HasValue ? eventDto.VerificationStatusId.Value : DBNull.Value);

                bool success = await _dataManager.ExecuteNonQueryAsync(procedureName, CommandType.StoredProcedure);
                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating event with ID {EventId}", eventId);
                return false;
            }
        }

        public async Task<bool> DeleteEventAsync(int eventId)
        {
            try
            {
                string query = "DELETE FROM events WHERE event_id = @p_event_id";
                _dataManager.ClearParameters();
                _dataManager.AddParameter("@p_event_id", eventId);
                bool success = await _dataManager.ExecuteNonQueryAsync(query, CommandType.Text);
                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting event with ID {EventId}", eventId);
                return false;
            }
        }

        private EventDTO MapEvent(DataRow row)
        {
            return new EventDTO
            {
                EventId = Convert.ToInt32(row["event_id"]),
                EventName = row["event_name"].ToString(),
                Description = row["description"].ToString(),
                StartDate = Convert.ToDateTime(row["start_date"]),
                EndDate = Convert.ToDateTime(row["end_date"]),
                Location = row["location"].ToString(),
                CommitteeId = Convert.ToInt32(row["committee_id"]),
                Tags = row["tags"] == DBNull.Value ? null : JsonDocument.Parse(row["tags"].ToString()),
                IsVerifiable = Convert.ToBoolean(row["isVerifiable"]),
                VerificationStatusId = row["verification_status_id"] == DBNull.Value ? null : Convert.ToInt32(row["verification_status_id"])
            };
        }
    }
}
