using FoodAPI.Entities;
using FoodAPI.Services;

namespace FoodAPI.Interfaces;

public interface IShippingInfoRepository
{
    Task<(IEnumerable<ShippingInfo>,PaginationMetadata)> GetAllUserOrderAsync(
        int userId, int pageNumber, int pageSize);

    Task<IEnumerable<ShippingInfo>> GetAllUserPendingOrdersAsync(int userId);
    Task<IEnumerable<ShippingInfo>> GetAllUserCompletedOrdersAsync(int userId);

    Task<int> GetTotalRestaurantOrderAsync(int restaurantId);
    Task<ShippingInfo?> GetShippingInfoDetailsAsync(int shippingInfoId);
    Task<bool> ShippingInfoExistsAsync(int shippingInfoId);
    Task<string> ShippingInfoBelongsToUserOrExistsAsync(int shippingInfoId, int? userId);
    Task CreateShippingInfoAsync(ShippingInfo shippingInfo);
    Task<bool> AddArrivedTimeAsync(int shippingInfoId);
    Task<bool> AddRatingAsync(int shippingInfoId, Rating rating);
    Task<bool> SaveChangesAsync();
}