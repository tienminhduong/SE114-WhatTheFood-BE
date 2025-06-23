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

    public async Task<Restaurant?> GetRestaurantAsync(int id)
    {
        return await foodOrderContext.Restaurants.FirstOrDefaultAsync(r => r.Id == id);
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