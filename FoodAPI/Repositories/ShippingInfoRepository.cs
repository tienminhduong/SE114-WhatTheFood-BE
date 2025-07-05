using FoodAPI.DbContexts;
using FoodAPI.Entities;
using FoodAPI.Interfaces;
using FoodAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace FoodAPI.Repositories;

public class ShippingInfoRepository(FoodOrderContext foodOrderContext) : IShippingInfoRepository
{
    public async Task<(IEnumerable<ShippingInfo>, PaginationMetadata)> GetAllUserOrderAsync(
        int userId, int pageNumber = 0, int pageSize = 10, string status = "")
    {
        var collection = foodOrderContext.ShippingInfos as IQueryable<ShippingInfo>;
        var totalItemCount = await collection.CountAsync();
        var paginationMetadata = new PaginationMetadata(
            totalItemCount, pageSize, pageNumber);

        var collectionToReturn = await collection
            .Where(si => si.UserId == userId)
            .Where(si => status == "" || si.Status == status)
            .Include(si => si.Restaurant)
                .ThenInclude(r => r!.Address)
            .Include(si => si.ShippingInfoDetails)
                .ThenInclude(sd => sd.FoodItem)
                    .ThenInclude(fi => fi!.FoodCategory)
            .Include(si => si.Address)
            .Include(si => si.Rating)
            .Include(si => si.User)
            .OrderBy(si => si.OrderTime)
            .Skip(pageSize * pageNumber)
            .Take(pageSize)
            .ToListAsync();
        return (collectionToReturn, paginationMetadata);
    }

    public async Task<IEnumerable<ShippingInfo>> GetAllOwnedOrder(int ownerId, string status = "")
    {
        var items = await foodOrderContext.ShippingInfos
            .Include(si => si.Restaurant)
                .ThenInclude(r => r!.Address)
            .Include(si => si.ShippingInfoDetails)
                .ThenInclude(sd => sd.FoodItem)
                    .ThenInclude(fi => fi!.FoodCategory)
            .Include(si => si.Address)
            .Include(si => si.Rating)
            .Include(si => si.User)
            .OrderBy(si => si.OrderTime)
            .Where(si => si.Restaurant!.OwnerId == ownerId)
            .Where(si => status == "" || si.Status == status)
            .ToListAsync();

        return items;
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
            .Include(si => si.ShippingInfoDetails).ThenInclude(sid => sid.FoodItem).ThenInclude(fi => fi!.FoodCategory)
            .Include(si => si.Restaurant).ThenInclude(r => r!.Address)
            .Include(si => si.Address)
            .Include(si => si.User)
            .FirstOrDefaultAsync(si => si.Id == shippingInfoId);
    }

    public async Task<bool> ShippingInfoExistsAsync(int shippingInfoId)
    {
        return await foodOrderContext.ShippingInfos.AnyAsync(si => si.Id == shippingInfoId); 
    }

    public async Task<string> ShippingInfoBelongsToUserOrExistsAsync(int shippingInfoId, int? userId)
    {
        bool valid = await  foodOrderContext.ShippingInfos.AnyAsync(si => si.Id == shippingInfoId);
        if(!valid)
            return String.Empty;
        if (userId == null)
            return "found";
        valid = await  foodOrderContext.ShippingInfos.AnyAsync(
            si => si.Id == shippingInfoId && si.UserId == userId);
        if (valid)
            return "matched";
        return "not matched";
    }

    public async Task CreateShippingInfoAsync(ShippingInfo shippingInfo)
    {
        shippingInfo.OrderTime = DateTime.Now;
        await foodOrderContext.ShippingInfos.AddAsync(shippingInfo);
    }

    public async Task<bool> AddRatingAsync(int shippingInfoId, Rating rating)
    {
        var shippingInfo = await foodOrderContext.ShippingInfos
            .FirstOrDefaultAsync(si => si.Id == shippingInfoId);

        var existedRating = await foodOrderContext.Ratings
            .FirstOrDefaultAsync(r => r.ShippingInfoId == shippingInfoId);
        if (existedRating != null)
            throw new Exception("All ready rated!");

        if (shippingInfo != null)
        {
            rating.RatingTime = DateTime.Now;
            rating.ShippingInfoId = shippingInfoId;
            await foodOrderContext.Ratings.AddAsync(rating);
            return true;
        }

        throw new Exception("No shipping info found");
    }

    public async Task<bool> SaveChangesAsync()
    {
        return (await foodOrderContext.SaveChangesAsync() > 0);
    }

    public async Task<ShippingInfo> GetAndValidateOwner(int shippingInfoId, int ownerId)
    {
        var shippingInfo = await foodOrderContext.ShippingInfos
            .Include(si => si.Restaurant)
                .ThenInclude(r => r!.Owner)
            .FirstOrDefaultAsync(si => si.Id == shippingInfoId);

        if (shippingInfo == null)
            throw new Exception("Shipping info not found");

        if (shippingInfo.Restaurant!.OwnerId != ownerId)
            throw new Exception("You don't own this restaurant");

        return shippingInfo;
    }

    public async Task<ShippingInfo> ApprovedOrder(int shippingInfoId, int ownerId)
    {
        var shippingInfo = await GetAndValidateOwner(shippingInfoId, ownerId);

        if (shippingInfo.Status != "Pending")
            throw new Exception("Not a pending order");

        shippingInfo.Status = "Approved";
        return shippingInfo;
    }

    public async Task<ShippingInfo> DeliverOrder(int shippingInfoId, int ownerId)
    {
        var shippingInfo = await GetAndValidateOwner(shippingInfoId, ownerId);
        if (shippingInfo.Status != "Approved")
            throw new Exception("Not an approved order");

        shippingInfo.Status = "Delivering";
        return shippingInfo;
    }

    public async Task<ShippingInfo> SetOrderDeliverd(int shippingInfoId, int ownerId)
    {
        var shippingInfo = await GetAndValidateOwner(shippingInfoId, ownerId);

        if (shippingInfo.Status != "Delivering")
            throw new Exception("Not a delivering order");

        shippingInfo.Status = "Delivered";
        shippingInfo.ArrivedTime = DateTime.Now;

        return shippingInfo;
    }

    public async Task<ShippingInfo> SetCompletedOrder(int shippingInfoId, int userId)
    {
        var shippingInfo = await foodOrderContext.ShippingInfos
            .Include(si => si.Restaurant)
                .ThenInclude(r => r!.Owner)
            .FirstOrDefaultAsync(si => si.Id == shippingInfoId)
            ?? throw new Exception("Shipping info not found");

        if (userId != shippingInfo.UserId)
            throw new Exception("You don't order this");

        if (shippingInfo.Status != "Delivered")
            throw new Exception("Order not delivered");

        shippingInfo.Status = "Completed";
        return shippingInfo;
    }
}