using FoodAPI.DbContexts;
using FoodAPI.Entities;
using FoodAPI.Interfaces;
using FoodAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FoodAPI.Services
{
    public class AuthService(IUserRepository userRepository, IConfiguration config) : IAuthService
    {
        public async Task<string?> LoginAsync(UserLoginDto userDto)
        {
            User? user = await userRepository.FindPhoneNumberExistsAsync(userDto.PhoneNumber);
            if (user == null)
                return null;

            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, userDto.Password)
                    == PasswordVerificationResult.Failed)
                return null;

            return CreateToken(user);
        }

        public async Task<User?> RegisterAsync(UserForCreationDto userDto)
        {
            User? u = await userRepository.FindPhoneNumberExistsAsync(userDto.PhoneNumber);
            if (u != null)
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

        private string CreateToken(User user)
        {
            List<Claim> claims = [
                new Claim(ClaimTypes.NameIdentifier, user.PhoneNumber),
                new Claim(ClaimTypes.Name, user.Name ?? ""),
                new Claim(ClaimTypes.Role, user.Role)
            ];

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetValue<string>("AppSettings:Token")!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescription = new JwtSecurityToken(
                issuer: config["AppSettings:Issuer"],
                audience: config["AppSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescription);
        }

        public async Task<string?> CheckOwnerPermission(string senderPhone, int restaurantId)
        {
            var sender = await userRepository.FindPhoneNumberExistsAsync(senderPhone);

            if (sender == null)
                return "Who tf are you?";

            var restaurant = sender.OwnedRestaurant.FirstOrDefault(r => r.Id == restaurantId);
            if (restaurant == null)
                return "You don't own this restaurant";

            return null;
        }
    }
}
