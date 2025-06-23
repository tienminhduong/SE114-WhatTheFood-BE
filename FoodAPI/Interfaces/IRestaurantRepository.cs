using FoodAPI.Entities;

namespace FoodAPI.Interfaces;

public interface IRestaurantRepository
{
    Task<IEnumerable<Restaurant>> GetRestaurantsAsync();
    Task<Restaurant?> GetRestaurantAsync(int id);
    Task CreateRestaurantAsync(Restaurant restaurant);
    Task<bool> SaveChangesAsync();
}