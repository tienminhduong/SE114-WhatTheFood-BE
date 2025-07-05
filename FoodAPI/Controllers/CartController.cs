using AutoMapper;
using FoodAPI.Entities;
using FoodAPI.Interfaces;
using FoodAPI.Models;
using Google.Apis.Upload;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
using System.Security.Claims;

namespace FoodAPI.Controllers;

[Route("api/cart")]
[ApiController]
[Authorize(Policy = "UserAccessLevel")]
public class CartController (
    ICartRepository cartRepository,
    IUserRepository userRepository,
    IMapper mapper
    )
    : ControllerBase
{
    private async Task<User?> FindUser()
    {
        var senderPhone = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var user = await userRepository.FindPhoneNumberExistsAsync(senderPhone);
        return user;
    }

    [HttpPost]
    public async Task<ActionResult> UpsertCartItem(int foodItemId, int amount = 0)
    {
        var user = await FindUser();

        if (user == null)
            return BadRequest("Who tf are you");

        await cartRepository.UpsertCartItem(user.Id, foodItemId, amount);
        await cartRepository.SaveChangeAsync();
        return Ok();
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteCartItem(int foodItemId)
    {
        var user = await FindUser();
        if (user == null)
            return BadRequest("Who tf are you");

        await cartRepository.DeleteCartItem(user.Id, foodItemId);
        await cartRepository.SaveChangeAsync();
        return NoContent();
    }

    [HttpDelete("ordered")]
    public async Task<ActionResult> DeleteOrderedCartItem(int restaurantId)
    {
        var user = await FindUser();
        if (user == null)
            return BadRequest("Who tf are you");

        await cartRepository.DeleteOrderedCardItem(user.Id, restaurantId);
        await cartRepository.SaveChangeAsync();
        return NoContent();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CartItemDto>>> GetAllCartItem()
    {
        var user = await FindUser();
        if (user == null)
            return BadRequest("Who tf are you");

        var list = await cartRepository.GetCartItemsByUserId(user.Id);
        var result = new List<CartItemDto>();
        CartItemDto? cartItem = null;

        foreach (var item in list)
        {
            cartItem ??= new CartItemDto
            {
                Restaurant = mapper.Map<RestaurantDto>(item.FoodItem!.Restaurant),
                TotalAmount = 0
            };

            if (cartItem.Restaurant!.Id != item.FoodItem!.RestaurantId)
            {
                result.Add(cartItem);
                cartItem = new CartItemDto
                {
                    Restaurant = mapper.Map<RestaurantDto>(item.FoodItem!.Restaurant),
                    TotalAmount = 0
                };
            }

            cartItem.OrderDetails.Add(new ShippingInfoDetailDto
            {
                Amount = item.Amount,
                FoodItem = mapper.Map<FoodItemDto>(item.FoodItem)
            });

            cartItem.TotalAmount += item.Amount * item.FoodItem.Price;
        }
        if (cartItem != null) result.Add(cartItem);

        return Ok(result);
    }

    [HttpGet("{restaurantId}")]
    public async Task<ActionResult<CartItemDto>> GetItemByRestaurantId(int restaurantId)
    {
        var user = await FindUser();
        if (user == null)
            return BadRequest("Who tf are you");

        var list = await cartRepository.GetCartItemsByRestaurantId(user.Id, restaurantId);
        var result = new CartItemDto
        {
            Restaurant = mapper.Map<RestaurantDto>(list.FirstOrDefault()?.FoodItem?.Restaurant),
            TotalAmount = 0
        };

        if (result.Restaurant == null)
            return BadRequest("No restaurant found");

        foreach (var item in list)
        {
            result.OrderDetails.Add(new ShippingInfoDetailDto
            {
                Amount = item.Amount,
                FoodItem = mapper.Map<FoodItemDto>(item.FoodItem)
            });
            result.TotalAmount += item.Amount * item.FoodItem!.Price;
        }

        return Ok(result);
    }
}
