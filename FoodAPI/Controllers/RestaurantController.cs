using System.Security.Claims;
using System.Text.Json;
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
    IFoodItemRepository foodItemRepository,
    IMapper mapper
    ) : ControllerBase
{
    private const int MaxRatingsPageSize = 20;

    [HttpGet]
    [Authorize(Policy = "UserAccessLevel")]
    public async Task<ActionResult<IEnumerable<RestaurantDto>>> GetAllRestaurants()
    {
        var restaurants = await restaurantRepository.GetRestaurantsAsync();
        return Ok(mapper.Map<IEnumerable<RestaurantDto>>(restaurants));
    }

    [HttpGet("detail/{id}", Name = "GetRestaurant")]
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
    [Authorize(Policy = "OwnerAccessLevel")]
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
    public async Task<ActionResult<IEnumerable<CreateRatingDto>>> GetRatingsByRestaurant(
        int restaurantId, int pageNumber = 1, int pageSize = 10)
    {
        if (pageSize >  MaxRatingsPageSize)
            pageSize = MaxRatingsPageSize;
        if(!(await restaurantRepository.CheckRestaurantExistAsync(restaurantId)))
            return NotFound();
        var (restaurantRatings, paginationMetadata) = await restaurantRepository
            .GetRatingsByRestaurantAsync(restaurantId, pageNumber, pageSize);
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(paginationMetadata));
        return Ok(mapper.Map<IEnumerable<RatingDto>>(restaurantRatings));
    }

    [HttpGet("{restaurantId}/orders")]
    public async Task<ActionResult<IEnumerable<ShippingInfoDto>>> GetAllOrders(int restaurantId)
    {
        try
        {
            var list = await restaurantRepository.GetOrderByRestaurant(restaurantId);
            return Ok(mapper.Map<IEnumerable<ShippingInfoDto>>(list));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{restaurantId}/orders/pending")]
    public async Task<ActionResult<IEnumerable<ShippingInfoDto>>> GetPendingOrders(int restaurantId)
    {
        try
        {
            var list = await restaurantRepository.GetOrderByRestaurant(restaurantId, "Pending");
            return Ok(mapper.Map<IEnumerable<ShippingInfoDto>>(list));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{restaurantId}/orders/delivering")]
    public async Task<ActionResult<IEnumerable<ShippingInfoDto>>> GetDeleveringOrders(int restaurantId)
    {
        try
        {
            var list = await restaurantRepository.GetOrderByRestaurant(restaurantId, "Delevering");
            return Ok(mapper.Map<IEnumerable<ShippingInfoDto>>(list));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{restaurantId}/orders/approved")]
    public async Task<ActionResult<IEnumerable<ShippingInfoDto>>> GetApprovedOrders(int restaurantId)
    {
        try
        {
            var list = await restaurantRepository.GetOrderByRestaurant(restaurantId, "Approved");
            return Ok(mapper.Map<IEnumerable<ShippingInfoDto>>(list));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{restaurantId}/orders/delivered")]
    public async Task<ActionResult<IEnumerable<ShippingInfoDto>>> GetDeliveredOrders(int restaurantId)
    {
        try
        {
            var list = await restaurantRepository.GetOrderByRestaurant(restaurantId, "Delivered");
            return Ok(mapper.Map<IEnumerable<ShippingInfoDto>>(list));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{restaurantId}/orders/completed")]
    public async Task<ActionResult<IEnumerable<ShippingInfoDto>>> GetCompletedOrder(int restaurantId)
    {
        try
        {
            var list = await restaurantRepository.GetOrderByRestaurant(restaurantId, "Completed");
            return Ok(mapper.Map<IEnumerable<ShippingInfoDto>>(list));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Policy = "OwnerAccessLevel")]
    [HttpGet("fooditems")]
    public async Task<ActionResult<IEnumerable<FoodItemDto>>> GetFoodItems()
    {
        try
        {
            string senderPhone = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var user = await userRepository.FindPhoneNumberExistsAsync(senderPhone);
            if (user == null)
                return BadRequest("Who tf are you");

            List<FoodItem> foodItems = [];
            foreach (var restaurant in user.OwnedRestaurant)
                foodItems.AddRange(await foodItemRepository.GetItemsForOwnedRestaurant(restaurant.Id));

            return Ok(mapper.Map<IEnumerable<FoodItemDto>>(foodItems));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}