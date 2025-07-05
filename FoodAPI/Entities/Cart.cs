using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FoodAPI.Entities
{
    [PrimaryKey(nameof(UserId), nameof(FoodItemId))]
    public class Cart
    {
        public int UserId { get; set; }
        public User? User { get; set; }
        public int FoodItemId { get; set; }
        public FoodItem? FoodItem { get; set; }
        public int Amount { get; set; }
    }
}
