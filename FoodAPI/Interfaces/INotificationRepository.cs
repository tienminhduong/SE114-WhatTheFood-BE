using FoodAPI.Entities;
using FoodAPI.Models;

namespace FoodAPI.Interfaces;

public interface INotificationRepository
{
    public Task<IEnumerable<Notification>> GetAllNotificationsAsync(int userId);
    public Task AddNewNotificationAsync(int userId, CreateNotificationDto notificationDto);
    public Task DeleteNotificationAsync(int notificationId);
    public Task<Notification> ReadNotificationAsync(int notificationId);
    public Task SaveChangeAsync();
}
