namespace FoodAPI.Models
{
    public class CreateFoodItemDto
    {
        public int RestaurantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public int Price { get; set; }
        public string ImgUrl { get; set; } = string.Empty;
    }
}
