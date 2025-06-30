namespace FoodAPI.Models;

public class NotificationDto
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public DateTime DateTime { get; set; }
    public bool IsRead { get; set; } = false;
}
