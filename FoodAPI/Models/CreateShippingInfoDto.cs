using System.ComponentModel.DataAnnotations;

namespace FoodAPI.Models;

public class CreateShippingInfoDto
{
    [Required]
    public int TotalPrice { get; set; }
    
    public string? UserNote { get; set; }
    
    [Required]
    public int RestaurantId { get; set; }
    
    [Required]
    public string PaymentMethod { get; set; } = String.Empty;
    
    [Required]
    public ICollection<CreateShippingInfoDetailDto> ShippingInfoDetails { get; set; } = new List<CreateShippingInfoDetailDto>();
    
    [Required]
    public AddressDto? Address { get; set; }
}