namespace FoodAPI.Models;

public class AddressDto
{
    public string Name { get; set; } = String.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}