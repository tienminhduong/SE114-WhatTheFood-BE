namespace FoodAPI.Models;

public class FoodItemRatingDto
{
    public int FoodItemId { get; set; }
    public FoodItemDto? FoodItem { get; set; }
    public int Number {  get; set; }
    public float AvgRating { get; set; }
}
