using System.ComponentModel.DataAnnotations;

namespace FoodAPI.Models;

public class CreateRestaurantDto
{
    [Required]
    public string name { get; set; }
    [Required] 
    public AddressDto? Address { get; set; }
}