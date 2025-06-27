using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodAPI.Entities;

public class FoodItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    [MaxLength(50)]
    public string FoodName { get; set; } = string.Empty;
    [MaxLength(200)]
    public string Description { get; set; } = string.Empty;
    public int SoldAmount { get; set; } = 0;
    public bool Available { get; set; } = true;
    public int Price { get; set; } = 0;
    public string? CldnrPublicId { get; set; }
    public string? CldnrUrl { get; set; }
    
    public int FoodCategoryId { get; set; }
    public FoodCategory? FoodCategory { get; set; }
    
    public int RestaurantId { get; set; }
    public Restaurant? Restaurant { get; set; }

    public ICollection<ShippingInfoDetail> ShippingInfoDetails { get; set; } =  new List<ShippingInfoDetail>();
}