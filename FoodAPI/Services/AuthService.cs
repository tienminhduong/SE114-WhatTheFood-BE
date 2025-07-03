using FoodAPI.Entities;
using FoodAPI.Interfaces;
using FoodAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FoodAPI.Services
{
    public class AuthService(IUserRepository userRepository, IConfiguration config) : IAuthService
    {
        public async Task<LoginTokenDto?> LoginAsync(UserLoginDto userDto)
        {
            User? user = await userRepository.FindPhoneNumberExistsAsync(userDto.PhoneNumber);
            if (user == null)
                return null;

            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, userDto.Password)
                    == PasswordVerificationResult.Failed)
                return null;

            return await CreateTokenAsync(user);
        }

        public async Task<User?> RegisterAsync(UserForCreationDto userDto)
        {
            User? u = await userRepository.FindPhoneNumberExistsAsync(userDto.PhoneNumber);
            if (u != null)
                throw new ArgumentException("Phone number already exists!");


            User user = new();
            string passwordHash = new PasswordHasher<User>().HashPassword(user, userDto.Password);

            user.PhoneNumber = userDto.PhoneNumber;
            user.PasswordHash = passwordHash;
            user.Name = userDto.Name;

            if (userDto.Role != "Owner" && userDto.Role != "User")
                throw new ArgumentException("Role not exists!");

            user.Role = userDto.Role;

            await userRepository.AddUserAsync(user);
            bool result = await userRepository.SaveChangesAsync();

            if (!result)
                throw new InvalidOperationException("Cannot create user!");

            return user;
        }

        private string CreateAccessToken(User user)
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
                expires: DateTime.UtcNow.AddDays(3),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(tokenDescription);
        }

        private async Task<LoginTokenDto> CreateTokenAsync(User user)
        {
            LoginTokenDto token = new()
            {
                AccessToken = CreateAccessToken(user),
                RefreshToken = await GenerateAndSaveRefreshTokenAsync(user)
            };

            return token;
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

        private async Task<User?> ValidateRefreshTokenAsync(string userPhone, string refreshToken)
        {
            var user = await userRepository.FindPhoneNumberExistsAsync(userPhone);
            if (user == null || user.RefreshToken != refreshToken
                || user.RefreshTokenExpiryTime <= DateTime.Now)
                return null;

            return user;
        }

        private static string GenerateRefreshToken()
        {
            var randomNumbers = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumbers);
            return Convert.ToBase64String(randomNumbers);
        }

        public async Task<string> GenerateAndSaveRefreshTokenAsync(User user)
        {
            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            await userRepository.SaveChangesAsync();
            return user.RefreshToken;
        }

        public async Task<LoginTokenDto?> RefreshTokenAsync(RefreshTokenDto refreshTokenDto)
        {
            var user = await ValidateRefreshTokenAsync
                (refreshTokenDto.PhoneNumber, refreshTokenDto.RefreshToken);
            if (user == null)
                return null;

            var token = new LoginTokenDto
            {
                AccessToken = CreateAccessToken(user),
                RefreshToken = user.RefreshToken
            };

            return token;
        }
    }
}
