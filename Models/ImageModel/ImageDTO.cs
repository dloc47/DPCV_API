
namespace DPCV_API.Models.ImageModel
{
    using System.ComponentModel.DataAnnotations;
    using System.Text.Json.Serialization;
    using global::DPCV_API.Models.ImageModel.DPCV_API.Models.ImageModel;

    namespace DPCV_API.Models.ImageModel
    {
        public class ImageDTO
        {
            public int ImageId { get; set; }

            [Required]
            public string ImageUrl { get; set; } = string.Empty;

            public string? OriginalImageName { get; set; }
            public string? ImageName { get; set; }
            public long? FileSize { get; set; }
            public string? MimeType { get; set; }

            [Required]
            public EntityTypeEnum EntityType { get; set; }

            [Required]
            public int EntityId { get; set; }

            public int? CommitteeId { get; set; }
            public int? UploadedBy { get; set; }
            public bool IsProfileImage { get; set; }

            [Required]
            public ImageStatusEnum Status { get; set; } = ImageStatusEnum.Active;

            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum EntityTypeEnum
        {
            Admin,
            Committee,
            Homestay,
            Activity,
            Event,
            Product
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum ImageStatusEnum
        {
            Active,
            Archived,
            Deleted
        }
    }



    public class UploadMediaRequest
    {
        [Required(ErrorMessage = "File is required.")]
        public IFormFile File { get; set; } // Renamed for clarity

        [Required(ErrorMessage = "Entity type is required.")]
        public EntityTypeEnum EntityType { get; set; } // Uses enum instead of string

        [Required(ErrorMessage = "Entity ID is required.")]
        public int EntityId { get; set; }

        public int? CommitteeId { get; set; } // Optional for committee users
    }


}