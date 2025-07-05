namespace FoodAPI.Models;

public class CartItemDto
{
    public RestaurantDto? Restaurant { get; set; }
    public List<ShippingInfoDetailDto> OrderDetails { get; set; } = [];
    public int TotalAmount { get; set; }
}
