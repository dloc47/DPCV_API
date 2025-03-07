using System.Text.Json;
using DPCV_API.Models.CommitteeModel;

namespace DPCV_API.Models.HomestayModel
{
    public class HomestayDTO
    {
        public int HomestayId { get; set; }
        public string HomestayName { get; set; } = string.Empty;
        public int CommitteeId { get; set; }
        public string Address { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string OwnerName { get; set; } = string.Empty;
        public string OwnerMobile { get; set; } = string.Empty;
        public int TotalRooms { get; set; }
        public decimal RoomTariff { get; set; }
        public List<string> Tags { get; set; } = new(); // JSON Array
        public List<string> Amenities { get; set; } = new(); // New JSON Array
        public string PaymentMethods { get; set; } = string.Empty; // (Stored as TEXT)
        public Dictionary<string, string> SocialMediaLinks { get; set; } = new(); // JSON Array
        public bool IsVerifiable { get; set; }
        public int? VerificationStatusId { get; set; }
        public int IsActive { get; set; }
    }

    public class HomestayResponseDTO : HomestayDTO
    {
        public int DistrictId { get; set; }
        public string DistrictName { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string? StatusType { get; set; }
    }
}
