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
    [Authorize(Policy = "UserAccessLevel")]
    public async Task<ActionResult<RestaurantDto>> GetRestaurant(int id, bool includeFoodItems = false)
    {
        var restaurant = await restaurantRepository.GetRestaurantByIdAsync(id, includeFoodItems);
        if (restaurant == null)
            return NotFound();
        if (includeFoodItems)
            return Ok(mapper.Map<RestaurantWithFoodsDto>(restaurant));
        return Ok(mapper.Map<RestaurantDto>(restaurant));
    }

    
    [HttpPost]
    [Authorize(Policy = "UserAccessLevel")]
    public async Task<ActionResult<RestaurantDto>> CreateRestaurant(CreateRestaurantDto restaurant)
    {
        string senderPhone = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var user = await userRepository.FindPhoneNumberExistsAsync(senderPhone);
        if (user == null)
            return NotFound();
        
        var restaurantEntity = mapper.Map<Restaurant>(restaurant);
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

    [HttpGet("{restaurantId}/ratings")]
    public async Task<ActionResult<IEnumerable<RatingDto>>> GetRatingsByRestaurant(int restaurantId)
    {
        if(!(await restaurantRepository.CheckRestaurantExistAsync(restaurantId)))
            return NotFound();
        var restaurantRatings = await restaurantRepository.GetRatingsByRestaurantAsync(restaurantId);
        return Ok(mapper.Map<IEnumerable<RatingDto>>(restaurantRatings));
    }
}