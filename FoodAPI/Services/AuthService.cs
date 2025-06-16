using FoodAPI.DbContexts;
using FoodAPI.Entities;
using FoodAPI.Interfaces;
using FoodAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FoodAPI.Services
{
    public class AuthService(IUserRepository userRepository, IConfiguration config) : IAuthService
    {
        public Task<string?> LoginAsync(UserLoginDto userDto)
        {
            throw new NotImplementedException();
        }

        public async Task<User?> RegisterAsync(UserForCreationDto userDto)
        {
            bool checkPhone = await userRepository.FindPhoneNumberExistsAsync(userDto.PhoneNumber);
            if (checkPhone)
                throw new ArgumentException("Phone number already exists!", nameof(userDto.PhoneNumber));


            User user = new();
            string passwordHash = new PasswordHasher<User>().HashPassword(user, userDto.Password);

            user.PhoneNumber = userDto.PhoneNumber;
            user.PasswordHash = passwordHash;
            user.Name = userDto.Name;

            userRepository.AddUser(user);
            bool result = await userRepository.SaveChangesAsync();

            if (!result)
                throw new InvalidOperationException("Cannot create user!");

            return user;
        }
    }
}
