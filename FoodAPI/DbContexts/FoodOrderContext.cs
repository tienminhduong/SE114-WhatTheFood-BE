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
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<NotificationToken> NotificationTokens { get; set; }
    public DbSet<Cart> Carts { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Restaurant>()
            .HasOne(r => r.Owner)
            .WithMany(o => o.OwnedRestaurant)
            .HasForeignKey(r => r.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FoodItem>()
            .HasOne(f => f.FoodCategory)
            .WithMany(fc => fc.FoodItems)
            .HasForeignKey(f => f.FoodCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FoodItem>()
            .HasOne(f => f.Restaurant)
            .WithMany(r => r.FoodItems)
            .HasForeignKey(f => f.RestaurantId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Notification>()
            .HasOne(n => n.User)
            .WithMany(u => u.Notifications)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableSensitiveDataLogging();
    }
}