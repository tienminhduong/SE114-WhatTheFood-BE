using FoodAPI.Entities;

namespace FoodAPI.Interfaces;

public interface IFoodCategoryRepository
{
    public Task<IEnumerable<FoodCategory>> GetCategoriesAsync();
    public Task<FoodCategory?> GetCategoryAsync(int userId);
    public Task AddCategoryAsync(User user);
    public Task<IEnumerable<FoodItem>> GetFoodOfCategoryAsync(int categoryId);
    Task<bool> SaveChangesAsync();
}