
namespace DPCV_API.Models.Users
{
    public class UserDTO
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public int? CommitteeId { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    //public class UserResponseDTO : UserDTO // ✅ If we want password in Response
    //{
    //    public string RoleName { get; set; } = string.Empty; // ✅ Non-nullable
    //    public string? CommitteeName { get; set; } // Can be null for Admin
    //    public int? DistrictId { get; set; } // Can be null for Admin
    //    public string? DistrictName { get; set; } // Can be null for Admin
    //    public string? Region { get; set; } = string.Empty; // Can be empty but not null
    //}

    public class UserResponseDTO // ✅ Response DTO should NOT contain the password
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public string? RoleName { get; set; }
        public int? CommitteeId { get; set; }
        public string? CommitteeName { get; set; }
        public int? DistrictId { get; set; }
        public string? DistrictName { get; set; }
        public string? Region { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

}
