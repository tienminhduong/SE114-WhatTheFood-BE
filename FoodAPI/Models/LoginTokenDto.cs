namespace FoodAPI.Models
{
    public class LoginTokenDto
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }
}
