using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodAPI.Entities;

public class Restaurant
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    
    public Address? Address { get; set; }
    
    public ICollection<FoodItem> FoodItems { get; set; } =  new List<FoodItem>();
    
    public ICollection<ShippingInfo> ShippingInfos { get; set; } =   new List<ShippingInfo>();
}