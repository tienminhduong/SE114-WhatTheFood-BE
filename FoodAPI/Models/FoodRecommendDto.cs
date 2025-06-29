namespace FoodAPI.Models
{
    public class FoodRecommendDto
    {
        public int FoodId { get; set; }
        public string? Name { get; set; }
        public float Rating { get; set; }
        public float DistanceInKm { get; set; }
        public float DistanceInTime { get; set; }
        public string? ImgUrl { get; set; }
    }
}
