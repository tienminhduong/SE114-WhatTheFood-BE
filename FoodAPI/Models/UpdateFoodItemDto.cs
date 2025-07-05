namespace FoodAPI.Models
{
    public class UpdateFoodItemDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public int Price { get; set; }
        public int SoldAmount { get; set; }
        public bool Available { get; set; }
        public string ImgUrl { get; set; } = string.Empty;
    }
}
