namespace DPCV_API.Models.CMS.Users
{
    public class AuthResponseDTO
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public int UserId { get; set; } // ✅ Matches `user_id`
        public int RoleId { get; set; } // ✅ Matches `role_id`
        public int? DistrictId { get; set; } // ✅ Matches `district_id`
        public bool IsActive { get; set; } // ✅ Matches `is_active`
        public DateTime CreatedAt { get; set; } // ✅ Matches `created_at`
        public DateTime UpdatedAt { get; set; } // ✅ Matches `updated_at`
    }
}
