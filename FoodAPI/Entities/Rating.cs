using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodAPI.Entities;

public class Rating
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    public short Star { get; set; }
    [Required]
    public DateTime RatingTime { get; set; }
    public string? Comment { get; set; }
    
    public int ShippingInfoId { get; set; }
    public ShippingInfo? ShippingInfo { get; set; }
}