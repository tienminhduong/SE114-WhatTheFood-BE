using AutoMapper;
using FoodAPI.Entities;
using FoodAPI.Interfaces;
using FoodAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodAPI.Controllers;

[Route("api/fooditems")]
[ApiController]
public class FoodItemController(
    IFoodItemRepository foodItemRepository,
    IFoodCategoryRepository foodCategoryRepository,
    IUserRepository userRepository,
    IMapper mapper) : ControllerBase
{
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
        return Ok(mapper.Map<FoodItemDto>(item));
    }

    [HttpPost]
    [Authorize(Policy = "OwnerAccessLevel")]
    public async Task<ActionResult<FoodItemDto>> CreateNewItem(CreateFoodItemDto itemDto)
    {
        var senderPhone = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var sender = await userRepository.FindPhoneNumberExistsAsync(senderPhone);

        if (sender == null)
            return BadRequest("Who tf are you?");

        var restaurant = sender.OwnedRestaurant.FirstOrDefault(r => r.Id == itemDto.RestaurantId);
        if (restaurant == null)
            return BadRequest("You don't own this restaurant");

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

        return Ok(mapper.Map<FoodItemDto>(item));
    }
}