using FirebaseAdmin.Messaging;
using FoodAPI.Interfaces;
using FoodAPI.Models;

namespace FoodAPI.Services
{
    public class NotificationService(
        IUserRepository userRepository,
        INotificationRepository notificationRepository
        ) : INotificationService
    {
        public async Task<string> SendNotification(int userId, CreateNotificationDto notificationDto)
        {
            var user = await userRepository.GetUserAsync(userId)
                ?? throw new Exception("No user found");

            await notificationRepository.AddNewNotificationAsync(userId, notificationDto);
            string responses = "";
            foreach (var token in user.NotificationTokens)
            {
                var message = new Message
                {
                    Notification = new Notification
                    {
                        Title = notificationDto.Title,
                        Body = notificationDto.Content,
                    },
                    Token = token.DeviceToken
                };
                var response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                responses += $"Token: {token.DeviceToken}, Response: {response}\n";
            }

            return responses;
        }
    }
}
