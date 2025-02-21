namespace DPCV_API.Models
{
    public class UserDTO
    {
        public int UserId { get; set; } // ✅ Matches `user_id`
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty; // ✅ Matches `password`
        public int RoleId { get; set; } // ✅ Matches `role_id`
        public string? RoleName { get; set; } // ✅ Optional, if you need role name
        public int? DistrictId { get; set; } // ✅ Matches `district_id`
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } // ✅ Matches `created_at`
        public DateTime UpdatedAt { get; set; } // ✅ Matches `updated_at`
    }
}
