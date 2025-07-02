using Microsoft.EntityFrameworkCore;

namespace FoodAPI.Entities;

[PrimaryKey(nameof(UserId), nameof(DeviceToken))]
public class NotificationToken
{
    public int UserId { get; set; }
    public string DeviceToken { get; set; } = string.Empty;
    public User? User { get; set; }
}
