namespace FoodAPI.Models;

public class ShippingInfoGetAllByUserDto
{
    public int Id { get; set; }
    public DateTime OrderTime { get; set; }
    public DateTime? ArrivedTime { get; set; }
    public int TotalPrice { get; set; }
    public RestaurantDto? RestaurantDto { get; set; }
    
}