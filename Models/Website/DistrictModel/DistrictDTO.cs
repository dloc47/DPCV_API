using System;

namespace DPCV_API.Models.Website.DistrictModel
{
    public class DistrictDTO
    {
        public int DistrictId { get; set; } // Matches `district_id`
        public string DistrictName { get; set; } = string.Empty; // Matches `district_name`
        public string Region { get; set; } = string.Empty; // Matches `region`
    }
}
