using System.Text.Json;

namespace DPCV_API.Models.CommitteeModel
{
    public class CommitteeDTO
    {
        public int CommitteeId { get; set; }
        public string CommitteeName { get; set; } = string.Empty;
        public int DistrictId { get; set; }
        public string? ContactNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public JsonDocument? Tags { get; set; } // JSON data type
        public bool IsVerifiable { get; set; }
        public int? VerificationStatusId { get; set; }
    }

    public class VillageDTO
    {
        public int CommitteeId { get; set; }
        public string CommitteeName { get; set; } = string.Empty;
        public int DistrictId { get; set; }
        public int? VerificationStatusId { get; set; }
    }
}
