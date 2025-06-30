using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodAPI.Entities;

public class ShippingInfo
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public DateTime OrderTime { get; set; }
    public DateTime? ArrivedTime { get; set; }
    [Required]
    public int TotalPrice { get; set; }
    [MaxLength(200)]
    public string? UserNote { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public string Status { get; set; } = "Pending";
    public int RestaurantId { get; set; }
    public Restaurant? Restaurant { get; set; }
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
    public ICollection<ShippingInfoDetail> ShippingInfoDetails { get; set; } = new List<ShippingInfoDetail>();
    public Address? Address { get; set; }
    public Rating? Rating { get; set; }
}