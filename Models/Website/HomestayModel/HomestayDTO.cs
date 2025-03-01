using System.Text.Json;

namespace DPCV_API.Models.Website.HomestayModel
{
    public class HomestayDTO
    {
        public int HomestayId { get; set; }
        public string HomestayName { get; set; } = string.Empty;
        public int CommitteeId { get; set; }
        public string Address { get; set; } = string.Empty;
        public string OwnerName { get; set; } = string.Empty;
        public string OwnerMobile { get; set; } = string.Empty;
        public int TotalRooms { get; set; }
        public decimal RoomTariff { get; set; }
        public JsonDocument? Tags { get; set; }
        public bool IsVerifiable { get; set; }
        public int? VerificationStatusId { get; set; }
        public int is_active { get; set; }
    }
}
