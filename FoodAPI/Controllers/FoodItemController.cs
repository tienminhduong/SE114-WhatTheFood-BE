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
    IUserRepository userRepository,
    IFoodCategoryRepository foodCategoryRepository,
    IAuthService authService,
    IMapRoutingService mapRoutingService,
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
            var senderPhone = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var user = await userRepository.FindPhoneNumberExistsAsync(senderPhone);
            if (user == null)
                return Unauthorized();

            var res = user.OwnedRestaurant.FirstOrDefault();
            int resId = res?.Id ?? 0;
                

            var fooditems = await foodItemRepository.GetItemsBy(
                pageNumber,
                pageSize,
                categoryId,
                nameContains,
                user.Role == "Owner" ? resId : restaurantId,
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
        var owner = await userRepository.FindPhoneNumberExistsAsync(senderPhone);
        if (owner == null)
            return BadRequest("How tf are you here");
        var restaurant = owner.OwnedRestaurant.FirstOrDefault();
        if (restaurant == null)
            return BadRequest("No restaurant owned, buy a new one");

        itemDto.RestaurantId = restaurant.Id;


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
            CldnrUrl = itemDto.ImgUrl
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
        foodItem.CldnrUrl = itemDto.ImgUrl;

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
    private static IEnumerable<FoodRecommendDto> SearchByRatingResult { get; set; } = [];

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

    [HttpGet("recommended/bysoldamount")]
    public async Task<ActionResult<IEnumerable<FoodRecommendDto>>> GetRecommdationBySoldAmount(
        int pageNumber = 0,
        int pageSize = 10
        )
    {
        try
        {
            List<FoodRecommendDto> result = [];
            var itemList = await foodItemRepository.GetItemsBy(
                pageNumber: pageNumber,
                pageSize: pageSize,
                sortBy: "soldAmountDesc");

            foreach (var item in itemList)
            {
                var frDto = new FoodRecommendDto
                {
                    FoodId = item.Id,
                    Name = item.FoodName,
                    ImgUrl = item.CldnrUrl,
                    SoldAmount = item.SoldAmount,
                    RestaurantName = item.Restaurant!.Name,
                    Rating = (await foodItemRepository.GetFoodItemAvgRating(item.Id)),
                };

                result.Add(frDto);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("recommended/byrating")]
    public async Task<ActionResult<IEnumerable<FoodRecommendDto>>> GetRecommendationByRating(
        int pageNumber = 0,
        int pageSize = 10
        )
    {
        try
        {
            if (pageNumber == 0)
            {
                List<FoodRecommendDto> l = [];
                var foodItems = (await foodItemRepository.GetItemsBy(pageSize: int.MaxValue))!;
                foreach (var item in foodItems)
                {
                    var frDto = new FoodRecommendDto
                    {
                        FoodId = item.Id,
                        Name = item.FoodName,
                        ImgUrl = item.CldnrUrl,
                        SoldAmount = item.SoldAmount,
                        RestaurantName = item.Restaurant!.Name,
                        Rating = (await foodItemRepository.GetFoodItemAvgRating(item.Id)),
                    };

                    l.Add(frDto);
                }

                SearchByRatingResult = l;
            }

            var result = SearchByRatingResult
                .OrderByDescending(x => x.Rating.AvgRating)
                    .ThenByDescending(x => x.Rating.Number)
                .Skip(pageNumber * pageSize).Take(pageSize)
                .ToList();

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}