using FoodAPI.Entities;
using FoodAPI.Models;

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
        Task AddFoodItem(FoodItemDto item);
        Task<bool> UpdateItem(FoodItem item);
        Task<bool> RemoveFoodItem(int id);
    }
}
