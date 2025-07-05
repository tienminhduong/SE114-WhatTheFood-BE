using AutoMapper;
using FirebaseAdmin.Auth;
using FirebaseAdmin.Messaging;
using FoodAPI.Entities;
using FoodAPI.Interfaces;
using FoodAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodAPI.Controllers;

[Route("api/users")]
[ApiController]
public class UserController(IUserRepository userRepository,
    IAuthService authService,
    INotificationRepository notificationRepository,
    IMapper mapper) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = "AdminAccessLevel")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        var userEntities = await userRepository.GetUsersAsync();
        return Ok(mapper.Map<IEnumerable<UserDto>>(userEntities));
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> RegisterUser(UserForCreationDto user)
    {
        try
        {
            User? newUser = await authService.RegisterAsync(user);
            if (newUser == null)
                return BadRequest("Register failed!");
            return Ok(mapper.Map<UserDto>(newUser));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginTokenDto>> LoginUser(UserLoginDto userLogin)
    {
        var token = await authService.LoginAsync(userLogin);
        if (token == null)
            return BadRequest("Username or password is not correct");

        return Ok(token);
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<LoginTokenDto>> RefreshToken(RefreshTokenDto refreshTokenDto)
    {
        var token = await authService.RefreshTokenAsync(refreshTokenDto);
        if (token == null)
            return BadRequest("Invalid refresh token");
        return Ok(token);
    }

    [Authorize]
    [HttpGet("info")]
    public async Task<ActionResult<UserDto>> CheckUser()
    {
        var userPhone = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var user = await userRepository.FindPhoneNumberExistsAsync(userPhone);
        if (user == null)
            return BadRequest("Who tf are you!");

        return Ok(mapper.Map<UserDto>(user));
    }

    [Authorize]
    [HttpGet("notifications")]
    public async Task<ActionResult<IEnumerable<NotificationDto>>> GetAllNotifications()
    {
        var userPhone = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var user = await userRepository.FindPhoneNumberExistsAsync(userPhone);
        if (user == null)
            return BadRequest("Who tf are you!");

        var result = await notificationRepository.GetAllNotificationsAsync(user.Id);
        return Ok(mapper.Map<IEnumerable<NotificationDto>>(result));
    }

    [Authorize]
    [HttpGet("notifications/{id}")]
    public async Task<ActionResult<IEnumerable<NotificationDto>>> ReadNotification(int id)
    {
        var userPhone = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var user = await userRepository.FindPhoneNumberExistsAsync(userPhone);
        if (user == null)
            return BadRequest("Who tf are you!");

        try
        {
            var result = await notificationRepository.ReadNotificationAsync(id);
            if (result == null)
                return NotFound();

            if (result.UserId != user.Id)
                return Forbid();

            return Ok(mapper.Map<NotificationDto>(result));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpDelete("notifications/{id}")]
    public async Task<ActionResult> DeleteNotification(int id)
    {
        var userPhone = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var user = await userRepository.FindPhoneNumberExistsAsync(userPhone);
        if (user == null)
            return BadRequest("Who tf are you!");

        try
        {
            var notification = await notificationRepository.ReadNotificationAsync(id);
            if (notification == null)
                return NotFound();

            if (notification.UserId != user.Id)
                return Forbid();


            await notificationRepository.DeleteNotificationAsync(id);
            await notificationRepository.SaveChangeAsync();
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpPost("device-token")]
    public async Task<ActionResult> AddDeviceToken([FromBody] NotificationTokenDto tokenDto)
    {
        try
        {
            var userPhone = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var user = await userRepository.FindPhoneNumberExistsAsync(userPhone);
            if (user == null)
                return BadRequest("Who tf are you!");


            await notificationRepository.AddNotificationToken(user.Id, tokenDto.DeviceToken);
            await notificationRepository.SaveChangeAsync();

            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpDelete("device-token")]
    public async Task<ActionResult> DeleteDeviceToken(string deviceToken)
    {
        try
        {
            var userPhone = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var user = await userRepository.FindPhoneNumberExistsAsync(userPhone);
            if (user == null)
                return BadRequest("Who tf are you!");
            await notificationRepository.DeleteUserToken(user.Id, deviceToken);
            await notificationRepository.SaveChangeAsync();
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpPost("fcm/test")]
    public async Task<ActionResult> TestFCM()
    {
        try
        {
            var userPhone = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var user = await userRepository.FindPhoneNumberExistsAsync(userPhone);
            if (user == null)
                return BadRequest("Who tf are you!");


            var tokens = await notificationRepository.GetUserTokens(user.Id);
            var responses = "";
            foreach (var token in tokens)
            {
                var message = new Message()
                {
                    Notification = new()
                    {
                        Title = "Vcl",
                        Body = "DDCaghwlghjkghekhgjkerhgk"
                    },
                    Token = token
                };

                string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                responses += $"Token: {token}, {response} \n";
            }
            return Ok(responses);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Policy = "OwnerAccessLevel")]
    [HttpGet("ownedrestaurant")]
    public async Task<ActionResult> GetOwnedRestaurants()
    {
        var userPhone = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var user = await userRepository.FindPhoneNumberExistsAsync(userPhone);
        if (user == null)
            return BadRequest("Who tf are you!");

        return Ok(new { restaurantId = user.OwnedRestaurant.FirstOrDefault()!.Id });
    }
}