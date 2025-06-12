using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodAPI.Entities;

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    [MaxLength(12)]
    public string PhoneNumber { get; set; } = String.Empty;
    [MaxLength(50)]
    public string? Name { get; set; }
    [Required]
    [MaxLength(20)]
    public string Password { get; set; } = string.Empty;
    
    public ICollection<Address> Addresses { get; set; } = new List<Address>();
    
    public ICollection<ShippingInfo> ShippingInfos { get; set; } = new List<ShippingInfo>();
}