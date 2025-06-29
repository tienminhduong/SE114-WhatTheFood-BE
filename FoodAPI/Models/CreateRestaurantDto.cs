using System.ComponentModel.DataAnnotations;

namespace FoodAPI.Models;

public class CreateRestaurantDto
{
    [Required]
    public required string Name { get; set; }
    [Required] 
    public AddressDto? Address { get; set; }
}