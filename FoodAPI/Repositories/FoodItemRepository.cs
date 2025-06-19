using FoodAPI.DbContexts;
using FoodAPI.Entities;
using FoodAPI.Interfaces;
using FoodAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodAPI.Repositories
{
    public class FoodItemRepository(FoodOrderContext dbContext) : IFoodItemRepository
    {
        public async Task AddFoodItemAsync(FoodItem item)
        { 
            await dbContext.AddAsync(item);
        }

        public async Task<FoodItem?> GetById(int id)
        {
            return await dbContext.FoodItems
                .Include(f => f.FoodCategory)
                .Include(f => f.Restaurant)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<IEnumerable<FoodItem>> GetItemsBy(
            int pageNumber = 0,
            int pageSize = 30,
            int categoryId = -1,
            string nameContains = "",
            int restaurantId = -1,
            bool isAvailableOnly = true,
            int priceLowerThan = 999,
            int priceHigherThan = 0,
            string sortBy = "")
        {
            int offset = pageNumber * pageSize;

            var foodItems = await dbContext.FoodItems
                .Where(food => categoryId == -1 || food.FoodCategoryId == categoryId)
                .Where(food => food.FoodName.Contains(nameContains))
                .Where(food => restaurantId == -1 || food.RestaurantId == restaurantId)
                .Where(food => !isAvailableOnly || food.Available)
                .Where(food => food.Price < priceLowerThan)
                .Where(food => food.Price > priceHigherThan)
                .Skip(offset).Take(pageSize)
                .Include(food => food.Restaurant)
                .Include(food => food.FoodCategory)
                .ToListAsync();

            switch (sortBy)
            {
                case "priceAsc":
                    foodItems = foodItems.OrderBy(f => f.Price).ToList();
                    break;
                case "priceDesc":
                    foodItems = foodItems.OrderByDescending(f => f.Price).ToList();
                    break;
                case "soldAmountAsc":
                    foodItems = foodItems.OrderBy(f => f.SoldAmount).ToList();
                    break;
                case "soldAmountDesc":
                    foodItems = foodItems.OrderByDescending(f => f.SoldAmount).ToList();
                    break;
                case "":
                    break;
                default:
                    throw new InvalidOperationException("Invalid sort criteria");
            }

            return foodItems;
        }

        public Task<bool> RemoveFoodItem(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SaveChangeAsync()
        {
            return await dbContext.SaveChangesAsync() > 0;
        }

        public Task<bool> UpdateItem(FoodItem item)
        {
            throw new NotImplementedException();
        }
    }
}
