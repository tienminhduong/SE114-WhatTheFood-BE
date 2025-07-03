using FoodAPI.Entities;
using FoodAPI.Services;

namespace FoodAPI.Interfaces;

public interface IRestaurantRepository
{
    Task<IEnumerable<Restaurant>> GetRestaurantsAsync();
    Task<Restaurant?> GetRestaurantByIdAsync(int id, bool includeFoodItems);
    Task CreateRestaurantAsync(Restaurant restaurant);
    Task<bool> CheckRestaurantExistAsync(int restaurantId);
    Task<(IEnumerable<Rating>,PaginationMetadata)> GetRatingsByRestaurantAsync(
        int restaurantId, int pageNumber, int pageSize);

    Task<IEnumerable<ShippingInfo>> GetOrderByRestaurant(int restaurantId, string status = "");
    Task<bool> SaveChangesAsync();
}