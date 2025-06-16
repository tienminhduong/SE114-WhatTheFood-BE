namespace FoodAPI.Models
{
    public class UserLoginDto
    {
        public required string PhoneNumber { get; set; }
        public required string Password { get; set; }
    }
}
