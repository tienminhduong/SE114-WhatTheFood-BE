using FoodAPI.Models;

namespace FoodAPI.Interfaces;

public interface INotificationService
{
    Task<string> SendNotification(int userId, CreateNotificationDto notificationDto);
}
