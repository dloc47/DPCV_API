using System.Text.Json;

namespace DPCV_API.Models.EventModel
{
    public class EventDTO
    {
        public int EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public int CommitteeId { get; set; }
        public JsonDocument? Tags { get; set; }
        public bool IsVerifiable { get; set; }
        public int? VerificationStatusId { get; set; }
        public int is_active { get; set; }
    }
}
