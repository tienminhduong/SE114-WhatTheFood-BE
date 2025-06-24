using AutoMapper;
using FoodAPI.Entities;
using FoodAPI.Models;

namespace FoodAPI.Profiles;

public class UserProfile: Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<UserForCreationDto, User>();
        CreateMap<FoodCategory, FoodCategoryDto>();
        CreateMap<FoodItem, FoodItemDto>();
        CreateMap<Restaurant, RestaurantDto>();
    }
}