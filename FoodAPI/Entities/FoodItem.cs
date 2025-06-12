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
    public string FoodName { get; set; }
    [MaxLength(200)]
    public string? Description { get; set; }
    public int? SoldAmount { get; set; }
    public bool? Available { get; set; }
    [Required]
    public int Price { get; set; }
    
    public int FoodCategoryId { get; set; }
    public FoodCategory? FoodCategory { get; set; }
    
    public int RestaurantId { get; set; }
    public Restaurant? Restaurant { get; set; }

    public ICollection<ShippingInfoDetail> ShippingInfoDetails { get; set; } =  new List<ShippingInfoDetail>();
}