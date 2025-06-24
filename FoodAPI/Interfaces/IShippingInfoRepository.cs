using FoodAPI.Entities;

namespace FoodAPI.Interfaces;

public interface IShippingInfoRepository
{
    Task<IEnumerable<ShippingInfo>> GetAllUserOrderAsync(int userId);
    Task<int> GetTotalRestaurantOrderAsync(int restaurantId);
    Task<ShippingInfo?> GetShippingInfoDetailsAsync(int shippingInfoId);
    Task<bool> ShippingInfoExistsAsync(int shippingInfoId);
    Task CreateShippingInfoAsync(ShippingInfo shippingInfo);
    Task AddArrivedTimeAsync(int shippingInfoId, DateTime arrivedTime);
    Task AddRatingAsync(int shippingInfoId, Rating rating);
    Task<bool> SaveChangesAsync();
}