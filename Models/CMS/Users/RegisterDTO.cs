namespace DPCV_API.Models.CMS.Users
{
    public class RegisterDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int RoleId { get; set; } // ✅ Matches `role_id`
        public int? DistrictId { get; set; } // ✅ Nullable for Admin users
    }
}
