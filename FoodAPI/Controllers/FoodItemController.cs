﻿using AutoMapper;
using FoodAPI.Entities;
using FoodAPI.Interfaces;
using FoodAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodAPI.Controllers;

[Route("api/fooditems")]
[ApiController]
public class FoodItemController(
    IFoodItemRepository foodItemRepository,
    IFoodCategoryRepository foodCategoryRepository,
    IAuthService authService,
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
}