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
[Route("api/shippinginfo")]
public class ShippingInfoController(
    IShippingInfoRepository shippingInfoRepository,
    IUserRepository userRepository,
    IRestaurantRepository restaurantRepository,
    IMapper mapper
    ) : ControllerBase
{
    private const int MaxShippingInfoPageSize = 20;

    
    [HttpGet]
    [Authorize(Policy = "UserAccessLevel")]
    public async Task<ActionResult<IEnumerable<ShippingInfo>>> GetAllByUser(
        int pageNumber = 1, int pageSize = 10)
    {
        if(pageSize > MaxShippingInfoPageSize)
            pageSize = MaxShippingInfoPageSize;
        string senderPhone = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var user = await userRepository.FindPhoneNumberExistsAsync(senderPhone);
        if (user == null)
            return NotFound();
        var (shippingInfo, paginationMetadata) = await shippingInfoRepository
            .GetAllUserOrderAsync(user.Id, pageNumber, pageSize);

        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(paginationMetadata));
        return Ok(mapper.Map<IEnumerable<ShippingInfoGetAllByUserDto>>(shippingInfo));
    }

    [HttpGet("pending")]
    [Authorize(Policy = "UserAccessLevel")]
    public async Task<ActionResult<IEnumerable<ShippingInfoDto>>> GetAllPendingOrdersByUser()
    {
        string senderPhone = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var user = await userRepository.FindPhoneNumberExistsAsync(senderPhone);
        if (user == null)
            return NotFound();
        var shippingInfoEntities = await shippingInfoRepository
            .GetAllUserPendingOrdersAsync(user.Id);
        return Ok(mapper.Map<IEnumerable<ShippingInfoGetAllByUserDto>>(shippingInfoEntities));
    } 
    
    [HttpGet("completed")]
    [Authorize(Policy = "UserAccessLevel")]
    public async Task<ActionResult<IEnumerable<ShippingInfoDto>>> GetAllCompletedOrdersByUser()
    {
        string senderPhone = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var user = await userRepository.FindPhoneNumberExistsAsync(senderPhone);
        if (user == null)
            return NotFound();
        var shippingInfoEntities = await shippingInfoRepository
            .GetAllUserCompletedOrdersAsync(user.Id);
        return Ok(mapper.Map<IEnumerable<ShippingInfoGetAllByUserDto>>(shippingInfoEntities));
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
        
        var shippingInfoEntity = mapper.Map<ShippingInfo>(createShippingInfoDto);
        shippingInfoEntity.UserId = user.Id;
        await shippingInfoRepository.CreateShippingInfoAsync(shippingInfoEntity);
        if (!(await shippingInfoRepository.SaveChangesAsync()))
            return BadRequest();
        
        var createdShippingInfo = mapper.Map<ShippingInfoDto>(shippingInfoEntity);
        
        return CreatedAtRoute("GetShippingInfoById",
            new { shippingInfoId = createdShippingInfo.Id },
            createdShippingInfo);
    }

    [HttpPost("order/{shippingInfoId}")]
    [Authorize(Policy = "UserAccessLevel")]
    public async Task<ActionResult> CompleteOrder(int shippingInfoId)
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

        await shippingInfoRepository.AddArrivedTimeAsync(shippingInfoId);
        if (!(await shippingInfoRepository.SaveChangesAsync()))
            return BadRequest();
        return NoContent();
    }

    [HttpPost("order/{shippingInfoId}/rating")]
    [Authorize(Policy = "UserAccessLevel")]
    public async Task<ActionResult> AddRating(int shippingInfoId, RatingDto ratingDto)
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
        await  shippingInfoRepository.AddRatingAsync(shippingInfoId, ratingEntity);
        if (!(await shippingInfoRepository.SaveChangesAsync()))
            return BadRequest();
        return Created();
    }
}