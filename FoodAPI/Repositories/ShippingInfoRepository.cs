using FoodAPI.DbContexts;
using FoodAPI.Entities;
using FoodAPI.Interfaces;
using FoodAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace FoodAPI.Repositories;

public class ShippingInfoRepository(FoodOrderContext foodOrderContext) : IShippingInfoRepository
{
    public async Task<(IEnumerable<ShippingInfo>,PaginationMetadata)> GetAllUserOrderAsync(
        int userId, int pageNumber, int pageSize)
    {
        var collection = foodOrderContext.ShippingInfos as  IQueryable<ShippingInfo>;
        var totalItemCount = await collection.CountAsync();
        var paginationMetadata = new PaginationMetadata(
            totalItemCount, pageSize, pageNumber);
        
        var collectionToReturn = await collection
            .OrderBy(si => si.OrderTime)
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToListAsync();
        return (collectionToReturn,paginationMetadata);
    }

    public async Task<IEnumerable<ShippingInfo>> GetAllUserPendingOrdersAsync(int userId)
    {
        return await foodOrderContext.ShippingInfos
            .Where(si => si.UserId == userId && si.ArrivedTime == null)
            .ToListAsync();
    }

    public async Task<IEnumerable<ShippingInfo>> GetAllUserCompletedOrdersAsync(int userId)
    {
        return await foodOrderContext.ShippingInfos
            .Where(si => si.UserId == userId && si.ArrivedTime != null)
            .ToListAsync();    
    }

    public async Task<int> GetTotalRestaurantOrderAsync(int restaurantId)
    {
        return await foodOrderContext.ShippingInfos
            .Where(si => si.RestaurantId == restaurantId)
            .CountAsync();
    }

    public async Task<ShippingInfo?> GetShippingInfoDetailsAsync(int shippingInfoId)
    {
        return await foodOrderContext.ShippingInfos
            .Include(si => si.ShippingInfoDetails).ThenInclude(sid => sid.FoodItem)
            .Include(si => si.Restaurant).ThenInclude(r => r!.Address)
            .Include(si => si.Address)
            .Include(si => si.Rating)
            .FirstOrDefaultAsync(si => si.Id == shippingInfoId);
    }

    public async Task<bool> ShippingInfoExistsAsync(int shippingInfoId)
    {
        return await  foodOrderContext.ShippingInfos.AnyAsync(si => si.Id == shippingInfoId); 
    }

    public async Task CreateShippingInfoAsync(ShippingInfo shippingInfo)
    {
        await foodOrderContext.ShippingInfos.AddAsync(shippingInfo);
    }

    public async Task AddArrivedTimeAsync(int shippingInfoId, DateTime arrivedTime)
    {
        var shippingInfo = await foodOrderContext.ShippingInfos.FindAsync(shippingInfoId);
        if (shippingInfo != null)
            shippingInfo.ArrivedTime = arrivedTime;
    }

    public async Task AddRatingAsync(int shippingInfoId, Rating rating)
    {
        var shippingInfo = await foodOrderContext.ShippingInfos.FindAsync(shippingInfoId);
        if (shippingInfo != null)
            shippingInfo.Rating = rating;
    }

    public async Task<bool> SaveChangesAsync()
    {
        return (await foodOrderContext.SaveChangesAsync() > 0);
    }
}