using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodAPI.Entities;

public class Notification
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public DateTime DateTime { get; set; }
    public bool IsRead { get; set; } = false;
}
