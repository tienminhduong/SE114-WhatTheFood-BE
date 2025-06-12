using FoodAPI.Entities;

namespace FoodAPI.Interfaces;

public interface IUserRepository
{
    public Task<IEnumerable<User>> GetUsersAsync();
    public Task<User?> GetUserAsync(int userId);
    public Task AddUserAsync(User user);
    
    Task<bool> SaveChangesAsync();
}