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
        public List<string> Tags { get; set; } = new(); // JSON Array
        public bool IsVerifiable { get; set; }
        public int? VerificationStatusId { get; set; }
        public int is_active { get; set; }
    }

    public class EventResponseDTO : EventDTO
    {
        public int DistrictId { get; set; }
        public string DistrictName { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string? StatusType { get; set; }
    }
}
