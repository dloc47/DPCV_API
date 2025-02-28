namespace DPCV_API.Models.ActivityModel
{
    public class ActivityDTO
    {
        public int ActivityId { get; set; }
        public string ActivityName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int CommitteeId { get; set; }
        public int? HomestayId { get; set; }
        public bool IsVerifiable { get; set; }
        public int? VerificationStatusId { get; set; }
        public int is_active { get; set; }
    }
}
