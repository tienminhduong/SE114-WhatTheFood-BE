namespace FoodAPI.Models;

public class ShippingInfoDto
{
    public int Id { get; set; }
    public DateTime OrderTime { get; set; }
    public DateTime? ArrivedTime { get; set; }
    public int TotalPrice { get; set; }
    public string? UserNote { get; set; }
    public RestaurantDto Restaurant { get; set; }

    public string PaymentMethod { get; set; } = String.Empty;
    
    public ICollection<ShippingInfoDetailDto> ShippingInfoDetails { get; set; } = new List<ShippingInfoDetailDto>();

    public AddressDto? Address { get; set; }

    public CreateRatingDto? Rating { get; set; }
    
}