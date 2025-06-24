﻿using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using FoodAPI.Entities;
using FoodAPI.Interfaces;
using FoodAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    
    [HttpGet("{userid}", Name = "GetUserById")]
    public async Task<ActionResult<UserDto>> GetUser(int userid)
    {
        var userEntity = await userRepository.GetUserAsync(userid);
        if (userEntity == null)
            return NotFound();

        return Ok(mapper.Map<UserDto>(userEntity));
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
    public async Task<ActionResult<string>> LoginUser(UserLoginDto userLogin)
    {
        string? token = await authService.LoginAsync(userLogin);
        if (token == null)
            return BadRequest("Username or password is not correct");

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

    [HttpPost("testimg")]
    public async Task<ActionResult<string>> TestUploadImg(IFormFile file)
    {
        Cloudinary cloudinary = new(Environment.GetEnvironmentVariable("CLOUDINARY_URL"));
        cloudinary.Api.Secure = true;

        using var stream = file.OpenReadStream();
        var uploadParams = new ImageUploadParams()
        {
            File = new FileDescription(file.FileName, stream),
            AssetFolder = "food"
        };
        var uploadResult = await cloudinary.UploadAsync(uploadParams);

        return Ok(uploadResult.SecureUrl.AbsoluteUri);
    }
}