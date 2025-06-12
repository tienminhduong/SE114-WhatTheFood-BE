using System.ComponentModel.DataAnnotations;

namespace FoodAPI.Models;

public class UserForCreationDto
{
    [Required]
    [MaxLength(12)]
    public string PhoneNumber { get; set; }

    [Required]
    [MaxLength(20)]
    public string Password { get; set; }

    public string? Name { get; set; }
}