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
        CreateMap<Entities.Restaurant, Models.RestaurantDto>()
            .ForMember(dest => dest.AddressDto,
                opt => opt.MapFrom(src => src.Address));
        CreateMap<Restaurant, RestaurantWithFoodsDto>();
        CreateMap<CreateRestaurantDto, Restaurant>();
        CreateMap<Address, AddressDto>();
    }
}