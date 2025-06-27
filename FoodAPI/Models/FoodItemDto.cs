namespace FoodAPI.Models
{
    public class FoodItemDto
    {
        public int Id { get; set; }
        public required string FoodName { get; set; }
        public string Description { get; set; } = string.Empty;
        public int SoldAmount { get; set; }
        public bool Available { get; set; }
        public int Price { get; set; }
        public string? CldnrUrl { get; set; }
        public FoodCategoryDto? FoodCategory { get; set; }
        public RestaurantDto? Restaurant { get; set; }
    }
}
