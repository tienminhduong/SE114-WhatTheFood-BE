using FoodAPI.Entities;

namespace FoodAPI.Interfaces;

public interface IShippingInfoRepository
{
    Task<IEnumerable<ShippingInfo>> GetAllUserOrderAsync();
    Task<int> GetTotalRestaurantOrderAsync(int restaurantId);
    Task CreateShippingInfoAsync(ShippingInfo shippingInfo);
    Task AddArrivedTimeAsync(int shippingInfoId, DateTime arrivedTime);
    Task AddRatingAsync(int shippingInfoId, Rating rating);
    Task<bool> SaveChangesAsync();
}