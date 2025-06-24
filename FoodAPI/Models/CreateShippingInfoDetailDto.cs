using System.ComponentModel.DataAnnotations;

namespace FoodAPI.Models;

public class CreateShippingInfoDetailDto
{
    [Required]
    public int FoodItemId { get; set; }
    [Required]
    public int Amount { get; set; }
}