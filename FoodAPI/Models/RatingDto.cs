using FoodAPI.Entities;

namespace FoodAPI.Models;

public class RatingDto
{
    public string? UserName { get; set; }
    public string? UserPfp { get; set; }
    public int Star {  get; set; }
    public string? Comment { get; set; }
    public ShippingInfo? ShippingInfo;
}
