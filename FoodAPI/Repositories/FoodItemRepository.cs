using FoodAPI.DbContexts;
using FoodAPI.Entities;
using FoodAPI.Interfaces;
using FoodAPI.Models;
using FoodAPI.Services;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

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
                    .ThenInclude(r => r!.Address)
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
                .Include(food => food.Restaurant)
                    .ThenInclude(res => res!.Address)
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
                    throw new InvalidEnumArgumentException("Invalid sort criteria");
            }

            return foodItems.Skip(offset).Take(pageSize).ToList();
        }

        public async Task<IEnumerable<FoodItem>> GetItemsForOwnedRestaurant(int restaurantId)
        {
            var items = await dbContext.FoodItems
                .Include(f => f.Restaurant)
                .Include(f => f.FoodCategory)
                .Where(f => f.RestaurantId ==  restaurantId)
                .ToListAsync();

            return items;
        }

        public Task<bool> RemoveFoodItem(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<(IEnumerable<Rating>,PaginationMetadata)> GetRatingsByFoodItemAsync(
            int foodItemId, int pageNumber, int pageSize)
        {
            var collection = dbContext.Ratings as IQueryable<Rating>;
            var totalItemCount = await collection.CountAsync();
            var paginationMetadata = new PaginationMetadata(totalItemCount, pageSize, pageNumber);
            var collectionToReturn = await collection
                .Where(r => r.ShippingInfo != null 
                            && r.ShippingInfo.ShippingInfoDetails.Any(sid => sid.FoodItemId == foodItemId))
                .OrderBy(r => r.RatingTime)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .Include(r => r.ShippingInfo)
                    .ThenInclude(si => si!.User)
                .ToListAsync();
            return (collectionToReturn, paginationMetadata);
        }

        public async Task<bool> FoodItemExistsAsync(int foodItemId)
        {
            return await dbContext.FoodItems.AnyAsync(f =>  f.Id == foodItemId);
        }

        public async Task<bool> SaveChangeAsync()
        {
            return await dbContext.SaveChangesAsync() > 0;
        }

        public Task<bool> UpdateItem(FoodItem item)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Rating>> GetRatingsByFoodItem(int foodItemId)
        {
            return await dbContext.Ratings
                .Include(r => r.ShippingInfo)
                    .ThenInclude(si => si!.ShippingInfoDetails)
                .Where(
                    r => r.ShippingInfo!.ShippingInfoDetails
                    .FirstOrDefault(sd => sd.FoodItemId == foodItemId) != null
                ).ToListAsync();
        }

        public async Task<FoodItemRatingDto> GetFoodItemAvgRating(int foodItemId)
        {
            var result = new FoodItemRatingDto
            {
                Number = 0,
                AvgRating = 0
            };

            var ratings = await GetRatingsByFoodItem(foodItemId);
            result.Number = ratings.Count();
            if (result.Number == 0)
                return result;

            result.AvgRating = (float)ratings.Sum(r => r.Star) / result.Number;

            return result;
        }
    }
}
