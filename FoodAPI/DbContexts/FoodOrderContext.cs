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
        modelBuilder.Entity<User>().HasData(
            new User()
            {
                Id = 1,
                PhoneNumber = "000000001",
                Name = "Yasuo",
                Password = "yasuoPassword"
            },
            new User()
            {
                Id = 2,
                PhoneNumber = "000000002",
                Name = "Leesin",
                Password = "LeesinPassword"
            },
            new User()
            {
                Id = 3,
                PhoneNumber = "000000003",
                Name = "Zed",
                Password = "zedPassword"
            }
            );
        base.OnModelCreating(modelBuilder);
    }
}