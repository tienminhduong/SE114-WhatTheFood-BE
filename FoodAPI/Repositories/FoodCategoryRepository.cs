using FoodAPI.DbContexts;
using FoodAPI.Entities;
using FoodAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodAPI.Repositories
{
    public class FoodCategoryRepository(FoodOrderContext dbContext) : IFoodCategoryRepository
    {
        public async Task AddCategoryAsync(string categoryName)
        {
            FoodCategory category = new();
            category.Name = categoryName;
            await dbContext.FoodCategories.AddAsync(category);
        }

        public async Task<IEnumerable<FoodCategory>> GetCategoriesAsync()
        {
            return await dbContext.FoodCategories.OrderBy(c => c.Name).ToListAsync();
        }

        public async Task<FoodCategory?> GetCategoryAsync(int id)
        {
            return await dbContext.FoodCategories.FirstOrDefaultAsync(fc => fc.Id == id);
        }

        public async Task<IEnumerable<FoodItem>> GetFoodOfCategoryAsync(int categoryId)
        {
            var category = await dbContext.FoodCategories.Include(fc => fc.FoodItems)
                .FirstOrDefaultAsync(fc => fc.Id == categoryId) ??
                throw new InvalidOperationException("No category found");

            return category.FoodItems;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await dbContext.SaveChangesAsync() > 0;
        }
    }
}
