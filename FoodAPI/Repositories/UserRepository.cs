using FoodAPI.DbContexts;
using FoodAPI.Entities;
using FoodAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodAPI.Repositories;

public class UserRepository(FoodOrderContext context) : IUserRepository
{
    private readonly FoodOrderContext _context = context ?? throw new ArgumentNullException(nameof(context));

    public async Task<IEnumerable<User>> GetUsersAsync()
    {
        return await _context.Users.OrderBy(u => u.Name).ToListAsync();
    }

    public async Task<User?> GetUserAsync(int userId)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId); 
    }

    public async Task AddUserAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return (await _context.SaveChangesAsync() > 0);
    }
}