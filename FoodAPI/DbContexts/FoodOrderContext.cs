using FoodAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodAPI.DbContexts;

public class FoodOrderContext(DbContextOptions<FoodOrderContext> options): DbContext(options)
{
    public DbSet<Address> Addresses { get; set; }
    public DbSet<FoodCategory> FoodCategories { get; set; }
    public DbSet<FoodItem> FoodItems { get; set; }
    public DbSet<Rating> Ratings { get; set; }
    public DbSet<Restaurant> Restaurants { get; set; }
    public DbSet<ShippingInfo> ShippingInfos { get; set; }
    public DbSet<ShippingInfoDetail> ShippingInfoDetails { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}