using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodAPI.Entities;

public class Address
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    [Required]
    public double Longitude { get; set; }
    [Required]
    public double Latitude { get; set; }
    [MaxLength(200)]
    public string? Note { get; set; }
    
    public int? UserId { get; set; }
    public User? User { get; set; }

    public int? RestaurantId { get; set; }
    public Restaurant? Restaurant { get; set; }
}