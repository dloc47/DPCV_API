namespace DPCV_API.Models.ActivityModel
{
    public class ActivityDTO
    {
        public int ActivityId { get; set; }
        public string ActivityName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<string> Tags { get; set; } = new(); // JSON Array
        public int CommitteeId { get; set; }
        public int? HomestayId { get; set; }
        public bool IsVerifiable { get; set; }
        public int? VerificationStatusId { get; set; }
        public int is_active { get; set; }
    }
    public class ActivityResponseDTO : ActivityDTO
    {
        public int DistrictId { get; set; }
        public string DistrictName { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string? StatusType { get; set; }
    }
}
