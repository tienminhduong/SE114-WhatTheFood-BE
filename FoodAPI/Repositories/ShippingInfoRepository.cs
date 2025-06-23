using FoodAPI.Entities;
using FoodAPI.Interfaces;

namespace FoodAPI.Repositories;

public class ShippingInfoRepository : IShippingInfoRepository
{
    public Task<IEnumerable<ShippingInfo>> GetAllUserOrderAsync()
    {
        throw new NotImplementedException();
    }

    public Task<int> GetTotalRestaurantOrderAsync(int restaurantId)
    {
        throw new NotImplementedException();
    }

    public Task CreateShippingInfoAsync(ShippingInfo shippingInfo)
    {
        throw new NotImplementedException();
    }

    public Task AddArrivedTimeAsync(int shippingInfoId, DateTime arrivedTime)
    {
        throw new NotImplementedException();
    }

    public Task AddRatingAsync(int shippingInfoId, Rating rating)
    {
        throw new NotImplementedException();
    }

    public Task<bool> SaveChangesAsync()
    {
        throw new NotImplementedException();
    }
}