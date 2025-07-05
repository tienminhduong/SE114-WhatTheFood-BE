using FoodAPI.DbContexts;
using FoodAPI.Entities;
using FoodAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodAPI.Repositories
{
    public class CartRepository(FoodOrderContext dbContext) : ICartRepository
    {
        public async Task DeleteCartItem(int userId, int foodItemId)
        {
            var item = await dbContext.Carts
                .FirstOrDefaultAsync(c => c.UserId == userId && c.FoodItemId == foodItemId)
                ?? throw new Exception("No cart item found");

            dbContext.Carts.Remove(item);
        }

        public async Task DeleteOrderedCardItem(int userId, int restaurantId)
        {
            var items = await dbContext.Carts
                .Include(c => c.FoodItem)
                .Where(c => c.FoodItem!.RestaurantId == restaurantId)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            dbContext.Carts.RemoveRange(items);
        }

        public async Task<IEnumerable<Cart>> GetCartItemsByRestaurantId(int userId, int restaurantId)
        {
            var result = await dbContext.Carts
                .Include(c => c.FoodItem)
                    .ThenInclude(fi => fi!.Restaurant)
                        .ThenInclude(r => r!.Address)
                .Where(c => c.UserId == userId)
                .Where(c => c.FoodItem!.RestaurantId == restaurantId)
                .ToListAsync();

            return result;
        }

        public async Task<IEnumerable<Cart>> GetCartItemsByUserId(int userId)
        {
            var result = await dbContext.Carts
                .Include(c => c.FoodItem)
                    .ThenInclude(fi => fi!.Restaurant)
                        .ThenInclude(r => r!.Address)
                .OrderBy(c => c.FoodItem!.RestaurantId)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            return result;
        }

        public async Task SaveChangeAsync()
        {
            await dbContext.SaveChangesAsync();
        }

        public async Task<Cart> UpsertCartItem(int userId, int foodItemId, int amount)
        {
            var item = await dbContext.Carts
                .FirstOrDefaultAsync(c => c.UserId == userId && c.FoodItemId == foodItemId);

            if (item == null)
            {
                item = new() { UserId = userId, FoodItemId = foodItemId, Amount = amount};
                await dbContext.Carts.AddAsync(item);
            }

            if (amount == 0)
                amount = item.Amount + 1;

            item.Amount = amount;
            return item;
        }
    }
}
