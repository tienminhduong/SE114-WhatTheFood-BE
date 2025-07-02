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

    public Task AddNotificationToken(int userId, string deviceToken);
    public Task<IEnumerable<string>> GetUserTokens(int userId);
    public Task DeleteUserToken(int userId, string deviceToken);
}
