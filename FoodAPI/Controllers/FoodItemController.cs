using AutoMapper;
using FoodAPI.Interfaces;
using FoodAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodAPI.Controllers;

[Route("api/fooditems")]
[ApiController]
[Authorize(Policy = "UserAccessLevel")]
public class FoodItemController(
    IFoodItemRepository foodItemRepository,
    IMapper mapper) : ControllerBase
{
    [HttpGet]
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
}