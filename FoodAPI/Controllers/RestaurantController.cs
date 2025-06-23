using System.Security.Claims;
using AutoMapper;
using FoodAPI.Entities;
using FoodAPI.Interfaces;
using FoodAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodAPI.Controllers;

[ApiController]
[Route("api/restaurants")]
public class RestaurantController(
    IRestaurantRepository restaurantRepository,
    IUserRepository userRepository,
    IMapper mapper,
    IAuthService authService
    ) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = "AdminAccessLevel")]
    public async Task<ActionResult<IEnumerable<RestaurantDto>>> GetAllRestaurants()
    {
        var restaurants = await restaurantRepository.GetRestaurantsAsync();
        return Ok(mapper.Map<IEnumerable<RestaurantDto>>(restaurants));
    }

    [HttpGet("{id}", Name = "GetRestaurant")]
    public async Task<ActionResult<RestaurantDto>> GetRestaurant(int id)
    {
        var restaurant = await restaurantRepository.GetRestaurantAsync(id);
        return Ok(mapper.Map<RestaurantDto>(restaurant));
    }

    
    [HttpPost]
    [Authorize(Policy = "UserAccessLevel")]
    public async Task<ActionResult<RestaurantDto>> CreateRestaurant(
        CreateRestaurantDto restaurant,
        string? longitude,
        string? latitude,
        string? addressName)
    {
        string senderPhone = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var user = await userRepository.FindPhoneNumberExistsAsync(senderPhone);
        if (user == null)
            return NotFound();
        var restaurantEntity = mapper.Map<Restaurant>(restaurant);
        if(addressName == null || longitude == null || latitude == null)
            return BadRequest();
        var addressEntity = new Address()
        {
            Name = addressName,
            Longitude = Convert.ToDouble(longitude),
            Latitude = Convert.ToDouble(latitude)
        };
        restaurantEntity.Address = addressEntity;
        restaurantEntity.OwnerId = user.Id;
        await restaurantRepository.CreateRestaurantAsync(restaurantEntity);
        if (!(await restaurantRepository.SaveChangesAsync()))
        {
            return BadRequest();
        }
        var createdRestaurant = mapper.Map<RestaurantDto>(restaurantEntity);
        return CreatedAtRoute("GetRestaurant",
            new { id = createdRestaurant.Id },
            createdRestaurant);
    }
}