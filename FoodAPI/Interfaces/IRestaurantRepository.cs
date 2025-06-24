using FoodAPI.Entities;

namespace FoodAPI.Interfaces;

public interface IRestaurantRepository
{
    Task<IEnumerable<Restaurant>> GetRestaurantsAsync();
    Task<Restaurant?> GetRestaurantByIdAsync(int id, bool includeFoodItems);
    Task CreateRestaurantAsync(Restaurant restaurant);
    Task<bool> CheckRestaurantExistAsync(int restaurantId);
    Task<bool> SaveChangesAsync();
}