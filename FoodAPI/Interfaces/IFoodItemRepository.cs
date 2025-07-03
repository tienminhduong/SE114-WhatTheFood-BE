using FoodAPI.Entities;
using FoodAPI.Models;
using FoodAPI.Services;

namespace FoodAPI.Interfaces
{
    public interface IFoodItemRepository
    {
        Task<IEnumerable<FoodItem>> GetItemsBy(
            int pageNumber = 0,
            int pageSize = 30,
            int categoryId = -1,
            string nameContains = "",
            int restaurantId = -1,
            bool isAvailableOnly = true,
            int priceLowerThan = int.MaxValue,
            int priceHigherThan = 0,
            string sortBy = ""
        );
        Task<FoodItem?> GetById(int id);
        Task AddFoodItemAsync(FoodItem item);
        Task<bool> UpdateItem(FoodItem item);
        Task<bool> RemoveFoodItem(int id);
        Task<(IEnumerable<Rating>,PaginationMetadata)> GetRatingsByFoodItemAsync(int foodItemId, int pageNumber, int pageSize);
        Task<bool> FoodItemExistsAsync(int foodItemId);
        Task<FoodItemRatingDto> GetFoodItemAvgRating(int foodItemId);
        Task<IEnumerable<Rating>> GetRatingsByFoodItem(int foodItemId);
        Task<IEnumerable<FoodItem>> GetItemsForOwnedRestaurant(int restaurantId);
        Task<bool> SaveChangeAsync();
    }
}
