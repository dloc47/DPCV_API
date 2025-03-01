namespace DPCV_API.Models.CMS.Users
{
    public class UserDTO
    {
        public int UserId { get; set; } // ✅ Matches `user_id`
        public string Name { get; set; } = string.Empty; // ✅ Matches `name`
        public string Email { get; set; } = string.Empty; // ✅ Matches `email`
        public string Password { get; set; } = string.Empty; // ✅ Matches `password`

        public int RoleId { get; set; } // ✅ Matches `role_id`
        public string? RoleName { get; set; } // ✅ Optional: If you need the role name

        public int? DistrictId { get; set; } // ✅ Matches `district_id`
        public string? DistrictName { get; set; } // ✅ Optional: If you need the district name

        public int? CommitteeId { get; set; } // ✅ Matches `committee_id`
        public string? CommitteeName { get; set; } // ✅ Optional: If you need the committee name

        public bool IsActive { get; set; } = true; // ✅ Matches `is_active`
        public DateTime CreatedAt { get; set; } // ✅ Matches `created_at`
        public DateTime UpdatedAt { get; set; } // ✅ Matches `updated_at`
    }
}
