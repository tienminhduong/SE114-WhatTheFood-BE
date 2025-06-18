using FoodAPI.DbContexts;
using FoodAPI.Entities;
using FoodAPI.Interfaces;

namespace FoodAPI.Repositories
{
    public class FoodCategoryRepository(FoodOrderContext dbContext) : IFoodCategoryRepository
    {
        public Task AddCategoryAsync(string categoryName)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<FoodCategory>> GetCategoriesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<FoodCategory?> GetCategoryAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<FoodItem>> GetFoodOfCategoryAsync(int categoryId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SaveChangesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
