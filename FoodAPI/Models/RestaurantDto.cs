namespace FoodAPI.Models
{
    public class RestaurantDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? CldnrUrl { get; set; }
        public AddressDto? AddressDto { get; set; }
    }
}
