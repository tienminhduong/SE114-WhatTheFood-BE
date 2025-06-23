namespace FoodAPI.Models;

public class RestaurantWithFoodsDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public AddressDto? AddressDto { get; set; }
    public ICollection<FoodItemDto> FoodItems { get; set; } = new List<FoodItemDto>();
}