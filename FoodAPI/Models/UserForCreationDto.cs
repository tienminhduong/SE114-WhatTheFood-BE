using System.ComponentModel.DataAnnotations;

namespace FoodAPI.Models;

public class UserForCreationDto
{
    [MaxLength(12)]
    public required string PhoneNumber { get; set; }

    public required string Password { get; set; }

    public string? Name { get; set; }
    public required string Role { get; set; }
}