using FoodAPI.DbContexts;
using FoodAPI.Entities;
using FoodAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodAPI.Repositories;

public class RestaurantRepository(FoodOrderContext foodOrderContext) : IRestaurantRepository
{
    public async Task<IEnumerable<Restaurant>> GetRestaurantsAsync()
    {
        return await foodOrderContext.Restaurants.ToListAsync();
    }

    public async Task<Restaurant?> GetRestaurantAsync(int id, bool includeFoodItems)
    {
        if (includeFoodItems)
        {
            return await foodOrderContext.Restaurants.Include(r => r.Address)
                .Include(r => r.FoodItems)
                .FirstOrDefaultAsync(r => r.Id == id);
        }
        return await foodOrderContext.Restaurants.Include(r => r.Address)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task CreateRestaurantAsync(Restaurant restaurant)
    {
        await foodOrderContext.Restaurants.AddAsync(restaurant);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return (await foodOrderContext.SaveChangesAsync()) > 0;
    }
}