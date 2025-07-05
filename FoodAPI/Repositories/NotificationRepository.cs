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

        public async Task AddNotificationToken(int userId, string deviceToken)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId)
                ?? throw new Exception("User not found");

            var t = await dbContext.NotificationTokens
                .FirstOrDefaultAsync(nt => nt.DeviceToken == deviceToken);

            //if (t != null)
            //throw new Exception("Device token already existed");

            if (t != null)
                dbContext.NotificationTokens.Remove(t);

            NotificationToken token = new()
            {
                UserId = userId,
                DeviceToken = deviceToken
            };

            await dbContext.AddAsync(token);
        }

        public async Task DeleteNotificationAsync(int notificationId)
        {
            var notification = await dbContext.Notifications
                .FirstOrDefaultAsync(u => u.Id == notificationId)
                ?? throw new Exception("No notification found");


            dbContext.Notifications.Remove(notification);
        }

        public async Task DeleteUserToken(int userId, string deviceToken)
        {
            var token = await dbContext.NotificationTokens
                .FirstOrDefaultAsync(nt => nt.DeviceToken == deviceToken)
                ?? throw new Exception("Device token not existed");

            if (token.UserId != userId)
                throw new Exception("Not your account");

            dbContext.NotificationTokens.Remove(token);
        }

        public async Task<IEnumerable<Notification>> GetAllNotificationsAsync(int userId)
        {
            var user = await dbContext.Users
                .Include(u => u.Notifications)
                .FirstOrDefaultAsync(u => u.Id == userId)
                ?? throw new Exception("No user found");

            return user.Notifications
                .OrderBy(n => n.IsRead)
                .OrderByDescending(n => n.DateTime)
                .ToList();
        }

        public async Task<IEnumerable<string>> GetUserTokens(int userId)
        {
            var user = await dbContext.Users
                .Include(u => u.NotificationTokens)
                .FirstOrDefaultAsync(u => u.Id == userId)
                ?? throw new Exception("User not found");

            return user.NotificationTokens
                .Select(nt => nt.DeviceToken)
                .AsEnumerable();
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
