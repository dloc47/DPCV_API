using System.Text.Json;

namespace DPCV_API.Models.Website.ProductModel
{
    public class ProductDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int CommitteeId { get; set; }
        public int? HomestayId { get; set; }
        public JsonDocument? Tags { get; set; }
        public bool IsVerifiable { get; set; }
        public int? VerificationStatusId { get; set; }
        public int is_active { get; set; }
    }
}
