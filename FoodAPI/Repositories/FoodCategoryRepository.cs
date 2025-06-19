using FoodAPI.DbContexts;
using FoodAPI.Entities;
using FoodAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodAPI.Repositories
{
    public class FoodCategoryRepository(FoodOrderContext dbContext) : IFoodCategoryRepository
    {
        public async Task<FoodCategory?> AddCategoryAsync(string categoryName)
        {
            await dbContext.FoodCategories.AddAsync(new FoodCategory { Name = categoryName });

            bool result = await SaveChangesAsync();
            if (!result)
                return null;

            var category = await GetCategoryByName(categoryName);
            return category;
        }

        public async Task<IEnumerable<FoodCategory>> GetCategoriesAsync()
        {
            return await dbContext.FoodCategories.OrderBy(c => c.Name).ToListAsync();
        }

        public async Task<FoodCategory?> GetCategoryAsync(int id)
        {
            return await dbContext.FoodCategories.FirstOrDefaultAsync(fc => fc.Id == id);
        }

        public async Task<FoodCategory?> GetCategoryByName(string categoryName)
        {
            return await dbContext.FoodCategories.FirstOrDefaultAsync(fc => fc.Name == categoryName);
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
