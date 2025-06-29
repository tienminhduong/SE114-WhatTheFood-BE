using AutoMapper;
using FoodAPI.Entities;
using FoodAPI.Interfaces;
using FoodAPI.Models;

namespace FoodAPI.Profiles;

public class UserProfile: Profile
{
    public UserProfile()
    {
        //User Mapper
        CreateMap<User, UserDto>();
        CreateMap<UserForCreationDto, User>();
        
        //Food Mapper
        CreateMap<FoodCategory, FoodCategoryDto>();
        CreateMap<FoodItem, FoodItemDto>();

        //Restaurant Mapper
        CreateMap<Restaurant, RestaurantDto>();
        CreateMap<Restaurant, RestaurantWithFoodsDto>();
        CreateMap<CreateRestaurantDto, Restaurant>();
        
        //Address Mapper
        CreateMap<Address, AddressDto>();
        CreateMap<AddressDto, Address>();

        //ShippingInfo Mapper
        CreateMap<ShippingInfo, ShippingInfoGetAllByUserDto>();
        CreateMap<ShippingInfoDetail, ShippingInfoDetailDto>();
        CreateMap<ShippingInfo, ShippingInfoDto>()
            .ForMember(dest => dest.PaymentMethod,
                opt => opt.MapFrom(src => src.PaymentMethod.ToString()));
        CreateMap<CreateShippingInfoDto, ShippingInfo>()
            .ForMember(dest => dest.PaymentMethod,
                opt => opt.MapFrom(src => Enum.Parse<PaymentMethod>(src.PaymentMethod)));
        CreateMap<CreateShippingInfoDetailDto, ShippingInfoDetail>();
        CreateMap<CreateRatingDto, Rating>();
        CreateMap<Rating, CreateRatingDto>();
        CreateMap<Rating, RatingDto>()
            .ForMember(dest => dest.UserName,
                opt => opt.MapFrom(src => src.ShippingInfo!.User!.Name))
            .ForMember(dest => dest.UserPfp,
                opt => opt.MapFrom(src => src.ShippingInfo!.User!.PfpUrl));
    }
}