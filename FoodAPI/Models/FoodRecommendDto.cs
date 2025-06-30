namespace FoodAPI.Models
{
    public class FoodRecommendDto
    {
        public int FoodId { get; set; }
        public string? Name { get; set; }
        public FoodItemRatingDto Rating { get; set; } = new() { AvgRating = 0, Number = 0 };
        public string? RestaurantName { get; set; }
        public int SoldAmount { get; set; } = 0;
        public float DistanceInKm { get; set; } = 0;
        public float DistanceInTime { get; set; } = 0;
        public string? ImgUrl { get; set; }
    }
}
