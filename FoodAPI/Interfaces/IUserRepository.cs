using FoodAPI.Entities;

namespace FoodAPI.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetUsersAsync();
    Task<User?> GetUserAsync(int userId);
    Task AddUserAsync(User user);
    void AddUser(User user);
    Task<bool> SaveChangesAsync();
    Task<bool> FindPhoneNumberExistsAsync(string phoneNumber);
}