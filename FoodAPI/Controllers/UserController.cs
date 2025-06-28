using AutoMapper;
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
    IMapper mapper): ControllerBase
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
        string? accessToken = await authService.LoginAsync(userLogin);
        if (accessToken == null)
            return BadRequest("Username or password is not correct");

        var token = new LoginTokenDto { AccessToken = accessToken };

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
}