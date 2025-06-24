namespace FoodAPI.Models;

public class UserDto
{
    public string PhoneNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? PfpUrl { get; set; }
}