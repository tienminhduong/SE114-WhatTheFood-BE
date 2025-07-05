using FoodAPI.Entities;
using FoodAPI.Services;

namespace FoodAPI.Interfaces;

public interface IShippingInfoRepository
{
    Task<(IEnumerable<ShippingInfo>, PaginationMetadata)> GetAllUserOrderAsync(
        int userId, int pageNumber = 0, int pageSize = 10, string status = "");

    //Task<IEnumerable<ShippingInfo>> GetAllUserPendingOrdersAsync(int userId);
    //Task<IEnumerable<ShippingInfo>> GetAllUserCompletedOrdersAsync(int userId);
    Task<IEnumerable<ShippingInfo>> GetAllOwnedOrder(int ownerId, string status = "");
    Task<int> GetTotalRestaurantOrderAsync(int restaurantId);
    Task<ShippingInfo?> GetShippingInfoDetailsAsync(int shippingInfoId);
    Task<bool> ShippingInfoExistsAsync(int shippingInfoId);
    Task<string> ShippingInfoBelongsToUserOrExistsAsync(int shippingInfoId, int? userId);
    Task CreateShippingInfoAsync(ShippingInfo shippingInfo);
    Task<ShippingInfo> ApprovedOrder(int shippingInfoId, int ownerId);
    Task<ShippingInfo> DeliverOrder(int shippingInfoId, int ownerId);
    Task<ShippingInfo> SetOrderDeliverd(int  shippingInfoId, int ownerId);
    Task<ShippingInfo> SetCompletedOrder(int shippingInfoId, int userId);
    Task<bool> AddRatingAsync(int shippingInfoId, Rating rating);
    Task<bool> SaveChangesAsync();
}