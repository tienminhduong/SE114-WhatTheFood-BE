namespace FoodAPI.Models
{
    public class RefreshTokenDto
    {
        public required string PhoneNumber { get; set; }
        public required string RefreshToken { get; set; }
    }
}
