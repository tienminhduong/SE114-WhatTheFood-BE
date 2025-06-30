using System.Security.Claims;
using System.Text.Json;
using AutoMapper;
using FoodAPI.Entities;
using FoodAPI.Interfaces;
using FoodAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace FoodAPI.Controllers;

[ApiController]
[Route("api/shippinginfo")]
public class ShippingInfoController(
    IShippingInfoRepository shippingInfoRepository,
    IUserRepository userRepository,
    IRestaurantRepository restaurantRepository,
    IFoodItemRepository foodItemRepository,
    INotificationRepository notificationRepository,
    IMapper mapper
    ) : ControllerBase
{
    private async Task<ActionResult<IEnumerable<ShippingInfoDto>>> GetOrders(
        int pageNumber = 0, int pageSize = 10, string status = "")
    {
        string senderPhone = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var user = await userRepository.FindPhoneNumberExistsAsync(senderPhone);

        if (user == null)
            return NotFound();

        var (shippingInfo, paginationMetadata) = await shippingInfoRepository
            .GetAllUserOrderAsync(user.Id, pageNumber, pageSize, status);

        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(paginationMetadata));
        return Ok(mapper.Map<IEnumerable<ShippingInfoDto>>(shippingInfo));
    }

    [HttpGet]
    [Authorize(Policy = "UserAccessLevel")]
    public async Task<ActionResult<IEnumerable<ShippingInfoDto>>> GetAllByUser(
        int pageNumber = 0, int pageSize = 10)
    {
        return await GetOrders(pageNumber, pageSize);
    }

    [HttpGet("pending")]
    [Authorize(Policy = "UserAccessLevel")]
    public async Task<ActionResult<IEnumerable<ShippingInfoDto>>> GetAllPendingOrdersByUser(
        int pageNumber = 0, int pageSize = 10)
    {
        return await GetOrders(pageNumber, pageSize, "Pending");
    }

    [HttpGet("approved")]
    [Authorize(Policy = "UserAccessLevel")]
    public async Task<ActionResult<IEnumerable<ShippingInfoDto>>> GetAllApprovedOrdersByUser(
        int pageNumber = 0, int pageSize = 10)
    {
        return await GetOrders(pageNumber, pageSize, "Approved");
    }

    [HttpGet("delivering")]
    [Authorize(Policy = "UserAccessLevel")]
    public async Task<ActionResult<IEnumerable<ShippingInfoDto>>> GetAllDeliveringOrdersByUser(
        int pageNumber = 0, int pageSize = 10)
    {
        return await GetOrders(pageNumber, pageSize, "Delivering");
    }

    [HttpGet("completed")]
    [Authorize(Policy = "UserAccessLevel")]
    public async Task<ActionResult<IEnumerable<ShippingInfoDto>>> GetAllCompletedOrdersByUser(
        int pageNumber = 0, int pageSize = 10)
    {
        return await GetOrders(pageNumber, pageSize, "Completed");
    }

    [HttpGet("{restaurantId}/total")]
    public async Task<ActionResult<int>> GetTotalRestaurantOrder(int restaurantId)
    {
        if(!(await restaurantRepository.CheckRestaurantExistAsync(restaurantId)))
            return NotFound();
        
        var total = await shippingInfoRepository.GetTotalRestaurantOrderAsync(restaurantId);
        return Ok(total);
    }

    [HttpGet("detail/{shippingInfoId}", Name = "GetShippingInfoById")]
    public async Task<ActionResult<ShippingInfo>> GetShippingInfoDetails(int shippingInfoId)
    {
        if(!(await shippingInfoRepository.ShippingInfoExistsAsync(shippingInfoId)))
            return NotFound();
        var shippingInfo = await shippingInfoRepository.GetShippingInfoDetailsAsync(shippingInfoId);
        return Ok(mapper.Map<ShippingInfoDto>(shippingInfo));
    }

    [HttpPost("order")]
    [Authorize(Policy = "UserAccessLevel")]
    public async Task<ActionResult<ShippingInfoDto>> CreateShippingInfo(CreateShippingInfoDto createShippingInfoDto)
    {
        string senderPhone = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var user = await userRepository.FindPhoneNumberExistsAsync(senderPhone);
        if (user == null)
            return NotFound();

        var restaurant = await restaurantRepository
            .GetRestaurantByIdAsync(createShippingInfoDto.RestaurantId, false);

        if (restaurant == null)
            return NotFound();

        var shippingInfoEntity = mapper.Map<ShippingInfo>(createShippingInfoDto);
        shippingInfoEntity.UserId = user.Id;

        shippingInfoEntity.TotalPrice = 0;
        foreach (var detail in createShippingInfoDto.ShippingInfoDetails)
        {
            var item = await foodItemRepository.GetById(detail.FoodItemId);
            shippingInfoEntity.TotalPrice += item!.Price * detail.Amount;
        }

        foreach (var detail in shippingInfoEntity.ShippingInfoDetails)
        {
            var item = await foodItemRepository.GetById(detail.FoodItemId);
            detail.FoodItemPriceAtOrderTime = item!.Price;
            item.SoldAmount += detail.Amount;
        }

        await shippingInfoRepository.CreateShippingInfoAsync(shippingInfoEntity);

        var notification = new CreateNotificationDto
        {
            Title = "Đơn hàng mới",
            Content = $"{user.Name} đã đặt một đơn hàng!"
        };

        await notificationRepository
            .AddNewNotificationAsync(restaurant.OwnerId, notification);

        if (!(await shippingInfoRepository.SaveChangesAsync()))
            return BadRequest();
        
        var createdShippingInfo = mapper.Map<ShippingInfoDto>(shippingInfoEntity);

        return CreatedAtRoute("GetShippingInfoById",
            new { shippingInfoId = createdShippingInfo.Id },
            createdShippingInfo);
    }

    [Authorize(Policy = "OwnerAccessLevel")]
    [HttpPost("{shippingInfoId}/approve")]
    public async Task<ActionResult> ApprovedOrder(int shippingInfoId)
    {
        try
        {
            string senderPhone = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var owner = await userRepository.FindPhoneNumberExistsAsync(senderPhone);

            if (owner == null)
                return BadRequest("Who are you");

            await shippingInfoRepository.ApprovedOrder(shippingInfoId, owner.Id);
            await shippingInfoRepository.SaveChangesAsync();
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Policy = "OwnerAccessLevel")]
    [HttpPost("{shippingInfoId}/deliver")]
    public async Task<ActionResult> DeliverOrder(int shippingInfoId)
    {
        try
        {
            string senderPhone = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var owner = await userRepository.FindPhoneNumberExistsAsync(senderPhone);

            if (owner == null)
                return BadRequest("Who are you");

            await shippingInfoRepository.DeliverOrder(shippingInfoId, owner.Id);
            await shippingInfoRepository.SaveChangesAsync();
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Policy = "OwnerAccessLevel")]
    [HttpPost("{shippingInfoId}/setdelivered")]
    public async Task<ActionResult> SetDeliveredOrder(int shippingInfoId)
    {
        try
        {
            string senderPhone = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var owner = await userRepository.FindPhoneNumberExistsAsync(senderPhone);

            if (owner == null)
                return BadRequest("Who are you");

            await shippingInfoRepository.SetOrderDeliverd(shippingInfoId, owner.Id);
            await shippingInfoRepository.SaveChangesAsync();
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Policy = "UserAccessLevel")]
    [HttpPost("{shippingInfoId}/setcompleted")]
    public async Task<ActionResult> SetCompletedOrder(int shippingInfoId)
    {
        try
        {
            string senderPhone = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var user = await userRepository.FindPhoneNumberExistsAsync(senderPhone);

            if (user == null)
                return BadRequest("Who are you");

            await shippingInfoRepository.SetCompletedOrder(shippingInfoId, user.Id);
            await shippingInfoRepository.SaveChangesAsync();
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{shippingInfoId}/rating")]
    [Authorize(Policy = "UserAccessLevel")]
    public async Task<ActionResult> AddRating(int shippingInfoId, CreateRatingDto ratingDto)
    {
        string senderPhone = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var user = await userRepository.FindPhoneNumberExistsAsync(senderPhone);
        if (user == null)
            return NotFound();
        var result = await shippingInfoRepository.ShippingInfoBelongsToUserOrExistsAsync(shippingInfoId, user.Id);
        switch (result)
        {
            case "":
                return NotFound();
            case "not matched":
                return Forbid();
        }
        var ratingEntity = mapper.Map<Rating>(ratingDto);
        try
        {
            if (!(await shippingInfoRepository.AddRatingAsync(shippingInfoId, ratingEntity)))
                return BadRequest("Hello");
            if (!(await shippingInfoRepository.SaveChangesAsync()))
                return BadRequest("Hello1");
            return Created();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}