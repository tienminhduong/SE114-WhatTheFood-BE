using FoodAPI.Entities;

namespace FoodAPI.Interfaces;

public interface ICartRepository
{
    Task<Cart> UpsertCartItem(int userId, int foodItemId, int amount);
    Task DeleteCartItem(int userId, int foodItemId);
    Task DeleteOrderedCardItem(int userId, int restaurantId);
    Task<IEnumerable<Cart>> GetCartItemsByUserId(int userId);
    Task<IEnumerable<Cart>> GetCartItemsByRestaurantId(int userId, int restaurantId);
    Task SaveChangeAsync();
}
