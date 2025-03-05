using System.Text.Json;

namespace DPCV_API.Models.CommitteeModel
{
    public class CommitteeDTO
    {
        public int CommitteeId { get; set; }
        public string CommitteeName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int DistrictId { get; set; }
        public string? ContactNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public List<string> Tags { get; set; } = new(); // JSON Array
        public List<TouristAttraction> TouristAttractions { get; set; } = new(); // JSON Array
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public List<LeadershipMember> Leadership { get; set; } = new(); // JSON Array
        public int IsVerifiable { get; set; }
        public int? VerificationStatusId { get; set; }
        public int IsActive { get; set; }
    }

    // Model for Nested Objects
    public class TouristAttraction
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class LeadershipMember
    {
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
    }

    public class CommitteeResponseDTO : CommitteeDTO
    {
        public string? VerificationStatus { get; set; }
        public string? DistrictName { get; set; }
    }



    public class VillageDTO
    {
        public int CommitteeId { get; set; }
        public string CommitteeName { get; set; } = string.Empty;
        public int DistrictId { get; set; }
        public int? VerificationStatusId { get; set; }
        public int is_active { get; set; }
    }
}
