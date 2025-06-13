using AutoMapper;
using FoodAPI.Entities;

namespace FoodAPI.Profiles;

public class UserProfile: Profile
{
    public UserProfile()
    {
        CreateMap<Entities.User, Models.UserDto>();
        CreateMap<Models.UserForCreationDto,Entities.User>();
    }
}