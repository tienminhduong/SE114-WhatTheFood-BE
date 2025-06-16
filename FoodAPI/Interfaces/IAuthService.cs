using FoodAPI.Entities;
using FoodAPI.Models;

namespace FoodAPI.Interfaces
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(UserForCreationDto userDto);
        Task<string?> LoginAsync(UserLoginDto userDto);
    }
}
