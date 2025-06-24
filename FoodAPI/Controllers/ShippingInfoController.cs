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
        int pageNumber = 10, int pageSize = 10)
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
        return Ok(mapper.Map<IEnumerable<ShippingInfo>>(shippingInfo));
    }

    [HttpGet("{restaurantId}/total")]
    public async Task<ActionResult<int>> GetTotalRestaurantOrder(int restaurantId)
    {
        if(!(await restaurantRepository.CheckRestaurantExistAsync(restaurantId)))
            return NotFound();
        return await shippingInfoRepository.GetTotalRestaurantOrderAsync(restaurantId);
    }

    [HttpGet("{shippingInfoId}", Name = "GetShippingInfoById")]
    public async Task<ActionResult<ShippingInfo>> GetShippingInfoDetails(int shippingInfoId)
    {
        if(!(await restaurantRepository.CheckRestaurantExistAsync(shippingInfoId)))
            return NotFound();
        var shippingInfo = shippingInfoRepository.GetShippingInfoDetailsAsync(shippingInfoId);
        return Ok(mapper.Map<ShippingInfoDetailDto>(shippingInfo));
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
    public async Task<ActionResult> CompleteOrder(int shippingInfoId, DateTime arrivedTime)
    {
        if(!(await shippingInfoRepository.ShippingInfoExistsAsync(shippingInfoId)))
            return NotFound();
        await shippingInfoRepository.AddArrivedTimeAsync(shippingInfoId, arrivedTime);
        if (!(await shippingInfoRepository.SaveChangesAsync()))
            return BadRequest();
        return NoContent();
    }

    [HttpPost("order/{shippingInfoId}/rating")]
    public async Task<ActionResult> AddRating(int shippingInfoId, RatingDto ratingDto)
    {
        if(!(await  shippingInfoRepository.ShippingInfoExistsAsync(shippingInfoId)))
            return NotFound();
        var ratingEntity = mapper.Map<Rating>(ratingDto);
        await  shippingInfoRepository.AddRatingAsync(shippingInfoId, ratingEntity);
        if (!(await shippingInfoRepository.SaveChangesAsync()))
            return BadRequest();
        return Created();
    }
}