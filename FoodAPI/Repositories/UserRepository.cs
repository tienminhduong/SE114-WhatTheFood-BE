﻿using FoodAPI.DbContexts;
using FoodAPI.Entities;
using FoodAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodAPI.Repositories;

public class UserRepository(FoodOrderContext context) : IUserRepository
{
    public async Task<IEnumerable<User>> GetUsersAsync()
    {
        return await context.Users.OrderBy(u => u.Name).ToListAsync();
    }

    public async Task<User?> GetUserAsync(int userId)
    {
        return await context.Users
            .Include(u => u.NotificationTokens)
            .FirstOrDefaultAsync(u => u.Id == userId); 
    }

    public async Task AddUserAsync(User user)
    {
        await context.Users.AddAsync(user);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return (await context.SaveChangesAsync() > 0);
    }

    public void AddUser(User user)
    {
        context.Users.Add(user);
    }

    public async Task<User?> FindPhoneNumberExistsAsync(string phoneNumber)
    {
        return await context.Users
            .Include(u => u.OwnedRestaurant)
            .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
    }
}