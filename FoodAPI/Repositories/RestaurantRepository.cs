using FoodAPI.DbContexts;
using FoodAPI.Entities;
using FoodAPI.Interfaces;
using FoodAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace FoodAPI.Repositories;

public class RestaurantRepository(FoodOrderContext foodOrderContext) : IRestaurantRepository
{
    public async Task<IEnumerable<Restaurant>> GetRestaurantsAsync()
    {
        return await foodOrderContext.Restaurants.Include(r => r.Address).ToListAsync();
    }

    public async Task<Restaurant?> GetRestaurantByIdAsync(int id, bool includeFoodItems)
    {
        if (includeFoodItems)
        {
            return await foodOrderContext.Restaurants.Include(r => r.Address)
                .Include(r => r.Owner)
                .Include(r => r.FoodItems)
                .FirstOrDefaultAsync(r => r.Id == id);
        }
        return await foodOrderContext.Restaurants.Include(r => r.Address)
            .Include(r => r.Owner)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task CreateRestaurantAsync(Restaurant restaurant)
    {
        await foodOrderContext.Restaurants.AddAsync(restaurant);
    }

    public async Task<bool> CheckRestaurantExistAsync(int restaurantId)
    {
        return await foodOrderContext.Restaurants.AnyAsync(r => r.Id == restaurantId);
    }

    public async Task<IEnumerable<ShippingInfo>> GetOrderByRestaurant(int restaurantId, string status = "")
    {
        return await foodOrderContext.ShippingInfos
            .Where(si => si.RestaurantId == restaurantId)
            .Where(si => status == "" || si.Status == status)
            .Include(si => si.Restaurant)
            .Include(si => si.Address)
            .Include(si => si.User)
            .Include(si => si.Rating)
            .Include(si => si.ShippingInfoDetails)
                .ThenInclude(sd => sd.FoodItem)
            .ToListAsync();
    }

    public async Task<(IEnumerable<Rating>,PaginationMetadata)> GetRatingsByRestaurantAsync(
        int restaurantId, int pageNumber, int pageSize)
    {
        var collections = foodOrderContext.Ratings as IQueryable<Rating>;
        var totalItemCount = await collections.CountAsync();
        var paginationMetadata = new PaginationMetadata(totalItemCount, pageSize, pageNumber);
        var collectionsToReturn = await collections
            .Where(r => r.ShippingInfo != null 
                        && r.ShippingInfo.RestaurantId == restaurantId)
            .OrderBy(r => r.RatingTime)
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .Include(r => r.ShippingInfo)
                .ThenInclude(si => si!.User)
            .ToListAsync();
        return (collectionsToReturn, paginationMetadata);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return (await foodOrderContext.SaveChangesAsync()) > 0;
    }
}