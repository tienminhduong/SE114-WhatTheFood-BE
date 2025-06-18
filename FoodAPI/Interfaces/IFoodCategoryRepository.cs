using FoodAPI.Entities;

namespace FoodAPI.Interfaces;

public interface IFoodCategoryRepository
{
    public Task<IEnumerable<FoodCategory>> GetCategoriesAsync();
    public Task<FoodCategory?> GetCategoryAsync(int id);
    public Task AddCategoryAsync(string categoryName);
    public Task<IEnumerable<FoodItem>> GetFoodOfCategoryAsync(int categoryId);
    Task<bool> SaveChangesAsync();
}