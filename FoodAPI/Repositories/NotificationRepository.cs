using FoodAPI.DbContexts;
using FoodAPI.Entities;
using FoodAPI.Interfaces;
using FoodAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodAPI.Repositories
{
    public class NotificationRepository(FoodOrderContext dbContext) : INotificationRepository
    {
        public async Task AddNewNotificationAsync(int userId, CreateNotificationDto notificationDto)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                throw new Exception("User not found");

            var notification = new Notification
            {
                UserId = user.Id,
                Title = notificationDto.Title,
                Content = notificationDto.Content,
                DateTime = DateTime.Now,
            };

            await dbContext.Notifications.AddAsync(notification);
        }

        public async Task DeleteNotificationAsync(int notificationId)
        {
            var notification = await dbContext.Notifications
                .FirstOrDefaultAsync(u => u.Id == notificationId)
                ?? throw new Exception("No notification found");


            dbContext.Notifications.Remove(notification);
        }

        public async Task<IEnumerable<Notification>> GetAllNotificationsAsync(int userId)
        {
            var user = await dbContext.Users
                .Include(u => u.Notifications)
                .FirstOrDefaultAsync(u => u.Id == userId)
                ?? throw new Exception("No user found");

            return user.Notifications.ToList();
        }

        public async Task<Notification> ReadNotificationAsync(int notificationId)
        {
            var notification = await dbContext.Notifications
                .FirstOrDefaultAsync(u => u.Id == notificationId)
                ?? throw new Exception("No notification found");

            notification.IsRead = true;
            await SaveChangeAsync();

            return notification;
        }

        public async Task SaveChangeAsync()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}
