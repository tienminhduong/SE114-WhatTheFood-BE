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
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "User";
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public string? PfpPublicId { get; set; }
    public string? PfpUrl { get; set; }
    public ICollection<Address> Addresses { get; set; } = new List<Address>();
    public ICollection<ShippingInfo> ShippingInfos { get; set; } = new List<ShippingInfo>();
    public ICollection<Restaurant> OwnedRestaurant { get; set; } = new List<Restaurant>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}