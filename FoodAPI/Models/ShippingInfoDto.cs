namespace FoodAPI.Models;

public class ShippingInfoDto
{
    public int Id { get; set; }
    public DateTime OrderTime { get; set; }
    public DateTime? ArrivedTime { get; set; }
    public int TotalPrice { get; set; }
    public string? UserNote { get; set; }
    public RestaurantDto? Restaurant { get; set; }
    public string Status { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public UserDto? User { get; set; }
    
    public IEnumerable<ShippingInfoDetailDto>? ShippingInfoDetails { get; set; }

    public AddressDto? Address { get; set; }

    public CreateRatingDto? Rating { get; set; }
    
}