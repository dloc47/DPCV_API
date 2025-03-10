using System.Text.Json;

namespace DPCV_API.Models.ProductModel
{
    public class ProductDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string MetricUnit { get; set; } = "Per Unit";
        public decimal? MetricValue { get; set; }
        public decimal Price { get; set; }
        public int CommitteeId { get; set; }
        public int? HomestayId { get; set; }
        public List<string> Tags { get; set; } = new();
        public bool IsVerifiable { get; set; }
        public int? VerificationStatusId { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class ProductResponseDTO : ProductDTO
    {
        public string CommitteeName { get; set; } = string.Empty;
        public string? HomestayName { get; set; } // Nullable since some products may not belong to a homestay
        public int DistrictId { get; set; }
        public string DistrictName { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string? StatusType { get; set; } // Mapped from verification_status_id if needed
    }

}

