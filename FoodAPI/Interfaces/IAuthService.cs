using FoodAPI.Entities;
using FoodAPI.Models;

namespace FoodAPI.Interfaces
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(UserForCreationDto userDto);
        Task<LoginTokenDto?> LoginAsync(UserLoginDto userDto);
        Task<LoginTokenDto?> RefreshTokenAsync(RefreshTokenDto refreshTokenDto);
        Task<string?> CheckOwnerPermission(string senderPhone, int restaurantId);
        Task<string> GenerateAndSaveRefreshTokenAsync(User user);
    }
}
