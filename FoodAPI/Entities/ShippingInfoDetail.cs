using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodAPI.Entities;

public class ShippingInfoDetail
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    public int Amount { get; set; }

    public int FoodItemPriceAtOrderTime { get; set; }
    
    public int ShippingInfoId { get; set; }
    public ShippingInfo? ShippingInfo { get; set; }

    public int FoodItemId { get; set; }
    public FoodItem? FoodItem { get; set; }
}