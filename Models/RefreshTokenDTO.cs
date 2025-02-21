namespace DPCV_API.Models
{
    public class RefreshTokenDTO
    {
        public string RefreshToken { get; set; } = string.Empty; // The refresh token itself
        public bool IsRevoked { get; set; }
    }
}
