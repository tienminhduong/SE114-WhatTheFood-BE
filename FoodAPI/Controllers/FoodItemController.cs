using AutoMapper;
using FoodAPI.Entities;
using FoodAPI.Interfaces;
using FoodAPI.Models;
using FoodAPI.Models.HEREDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace FoodAPI.Controllers;

[Route("api/fooditems")]
[ApiController]
public class FoodItemController(
    IFoodItemRepository foodItemRepository,
    IFoodCategoryRepository foodCategoryRepository,
    IAuthService authService,
    IMapRoutingService mapRoutingService,
    IRestaurantRepository restaurantRepository,
    IMapper mapper) : ControllerBase
{
    private const int MaxRatingsPageSize = 20;

    
    [HttpGet]
    [Authorize(Policy = "UserAccessLevel")]
    public async Task<ActionResult<IEnumerable<FoodItemDto>>> GetItemBy(
        int pageNumber = 0,
        int pageSize = 30,
        int categoryId = -1,
        string nameContains = "",
        int restaurantId = -1,
        bool isAvailableOnly = true,
        int priceLowerThan = int.MaxValue,
        int priceHigherThan = 0,
        string sortBy = "")
    {
        try
        {
            var fooditems = await foodItemRepository.GetItemsBy(
                pageNumber,
                pageSize,
                categoryId,
                nameContains,
                restaurantId,
                isAvailableOnly,
                priceLowerThan,
                priceHigherThan,
                sortBy);

            return Ok(mapper.Map<IEnumerable<FoodItemDto>>(fooditems));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "UserAccessLevel")]
    public async Task<ActionResult<FoodItemDto>> GetById(int id)
    {
        var item = await foodItemRepository.GetById(id);

        if (item == null)
            return NotFound("No food item with that id");

        return Ok(mapper.Map<FoodItemDto>(item));
    }

    [HttpPost]
    [Authorize(Policy = "OwnerAccessLevel")]
    public async Task<ActionResult<FoodItemDto>> CreateNewItem(CreateFoodItemDto itemDto)
    {
        string senderPhone = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        string? ownerPermissionCheckMsg =
            await authService.CheckOwnerPermission(senderPhone, itemDto.RestaurantId);

        if (ownerPermissionCheckMsg != null)
            return BadRequest(ownerPermissionCheckMsg);

        var category = await foodCategoryRepository.GetCategoryByName(itemDto.CategoryName);
        category ??= await foodCategoryRepository.AddCategoryAsync(itemDto.CategoryName);

        if (category == null)
            return BadRequest("Error on creating new category");


        var item = new FoodItem
        {
            FoodName = itemDto.Name,
            Description = itemDto.Description,
            SoldAmount = 0,
            Available = true,
            Price = itemDto.Price,
            FoodCategoryId = category.Id,
            RestaurantId = itemDto.RestaurantId,
        };

        await foodItemRepository.AddFoodItemAsync(item);
        bool result = await foodItemRepository.SaveChangeAsync();
        if (!result)
            return BadRequest("Error on saving");

        return CreatedAtAction(nameof(GetById), new {id = item.Id} , mapper.Map<FoodItemDto>(item));
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "OwnerAccessLevel")]
    public async Task<ActionResult<FoodItemDto>> UpdateFoodItem(int id, [FromBody]UpdateFoodItemDto itemDto)
    {
        var foodItem = await foodItemRepository.GetById(id);
        if (foodItem == null)
            return NotFound("Food item not found");

        string senderPhone = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        string? checkPermissionMsg =
            await authService.CheckOwnerPermission(senderPhone, foodItem.RestaurantId);

        if (checkPermissionMsg != null)
            return BadRequest(checkPermissionMsg);

        foodItem.FoodName = itemDto.Name;
        foodItem.Description = itemDto.Description;
        foodItem.Price = itemDto.Price;
        foodItem.SoldAmount = itemDto.SoldAmount;
        foodItem.Available = itemDto.Available;

        var category = await foodCategoryRepository.GetCategoryByName(itemDto.CategoryName);
        category ??= await foodCategoryRepository.AddCategoryAsync(itemDto.CategoryName);

        if (category == null)
            return BadRequest("Error on creating new category");

        foodItem.FoodCategoryId = category.Id;
        await foodItemRepository.SaveChangeAsync();

        return new EmptyResult();
    }

    [HttpGet("{id}/ratings")]
    public async Task<ActionResult<IEnumerable<CreateRatingDto>>> GetRatingsByFoodItem(
        int id, int pageSize = 10, int pageNumber = 1)
    {
        if(pageSize > MaxRatingsPageSize)
            pageSize = MaxRatingsPageSize;
        if(!(await foodItemRepository.FoodItemExistsAsync(id)))
            return NotFound();
        var (ratings, paginationMetadata) = await foodItemRepository
            .GetRatingsByFoodItemAsync(id, pageNumber, pageSize);
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

        return Ok(mapper.Map<IEnumerable<RatingDto>>(ratings));
    }

    [HttpGet("{id}/ratings/summary")]
    public async Task<ActionResult<FoodItemRatingDto>> TestGetRatings(int id)
    {
        try
        {
            FoodItemRatingDto ratings = await foodItemRepository.GetFoodItemAvgRating(id);
            return Ok(ratings);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    private static IEnumerable<FoodRecommendDto> SearchByLocationResult { get; set; } = [];

    [HttpGet("recommended/bylocation")]
    public async Task<ActionResult<IEnumerable<FoodRecommendDto>>> GetRecommendationByLocation(
        double latitude = 0,
        double longitude = 0,
        int pageNumber = 0,
        int pageSize = 10
        )
    {
        try
        {
            if (pageNumber == 0)
                SearchByLocationResult = await mapRoutingService
                    .GetRecommendFoodByLocation(latitude, longitude);

            var result = SearchByLocationResult
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}