using AutoMapper;
using FoodAPI.Entities;
using FoodAPI.Interfaces;
using FoodAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodAPI.Controllers;

[Route("api/images")]
[ApiController]
public class ImageController(
    IImageService imageService,
    IAuthService authService,
    IUserRepository userRepository,
    IRestaurantRepository restaurantRepository,
    IFoodItemRepository foodItemRepository,
    IMapper mapper
    ) : ControllerBase
{
    [Authorize(Policy = "UserAccessLevel")]
    [HttpPost("profile")]
    public async Task<ActionResult<UserDto>> UploadUserProfileImage(IFormFile image)
    {
        string senderPhone = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var user = await userRepository.FindPhoneNumberExistsAsync(senderPhone);

        if (user == null)
            return BadRequest("Who tf are you");

        if (user.PfpPublicId != null)
        {
            var deleteResult = await imageService.DeleteImageAsync(user.PfpPublicId);
            if (deleteResult.Error != null)
                return BadRequest(deleteResult.Error.Message);

            user.PfpUrl = user.PfpPublicId = null;
        }

        var uploadResult = await imageService.AddImageAsync(image, 300, 300);
        if (uploadResult.Error != null)
            return BadRequest(uploadResult.Error.Message);

        user.PfpPublicId = uploadResult.PublicId;
        user.PfpUrl = uploadResult.SecureUrl.AbsoluteUri;

        await userRepository.SaveChangesAsync();
        return Ok(mapper.Map<UserDto>(user));
    }

    [Authorize(Policy = "UserAccessLevel")]
    [HttpDelete("profile")]
    public async Task<ActionResult> DeleteUserProfileImg()
    {
        string senderPhone = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var user = await userRepository.FindPhoneNumberExistsAsync(senderPhone);

        if (user == null)
            return BadRequest("Who tf are you");

        if (user.PfpPublicId != null)
        {
            var deleteResult = await imageService.DeleteImageAsync(user.PfpPublicId);
            if (deleteResult.Error != null)
                return BadRequest(deleteResult.Error.Message);

            user.PfpUrl = user.PfpPublicId = null;
        }
        await userRepository.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("restaurant")]
    [Authorize(Policy = "OwnerAccessLevel")]
    public async Task<ActionResult<RestaurantDto>> UploadRestaurantImage(IFormFile image, int  restaurantId)
    {
        string senderPhone = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        string? ownerPermissionCheckMsg =
            await authService.CheckOwnerPermission(senderPhone, restaurantId);

        if (ownerPermissionCheckMsg != null)
            return BadRequest(ownerPermissionCheckMsg);
        var restaurantEntity = await restaurantRepository.GetRestaurantByIdAsync(restaurantId, false);
        //Already checked for that restaurant id exist by the authService but if this if condition is true, i dont know what to say @@@
        if (restaurantEntity == null)
            return BadRequest();

        if (restaurantEntity.CldnrPublicId != null)
        {
            var deleteResult = await imageService.DeleteImageAsync(restaurantEntity.CldnrPublicId);
            if(deleteResult.Error != null)
                return BadRequest(deleteResult.Error.Message);
            restaurantEntity.CldnrPublicId = restaurantEntity.CldnrUrl = null;
        }
        
        var uploadResult = await imageService.AddImageAsync(image, 640, 480);
        if (uploadResult.Error != null)
            return BadRequest(uploadResult.Error.Message);

        restaurantEntity.CldnrPublicId = uploadResult.PublicId;
        restaurantEntity.CldnrUrl = uploadResult.SecureUrl.AbsoluteUri;

        await restaurantRepository.SaveChangesAsync();
        return Ok(mapper.Map<RestaurantDto>(restaurantEntity));
    }

    [HttpPost("custom/food")]
    public async Task<ActionResult> UploadImage(IFormFile image)
    {
        var uploadResult = await imageService.AddImageAsync(image, 300, 300);
        if (uploadResult.Error != null)
            return BadRequest(uploadResult.Error.Message);

        return Ok(new { url = uploadResult.SecureUrl.AbsoluteUri });
    }
    
    [HttpPost("fooditem")]
    [Authorize(Policy = "OwnerAccessLevel")]
    public async Task<ActionResult<FoodItemDto>> UploadFoodItemImage(IFormFile image, int foodItemId)
    {
        var foodItemEntity = await foodItemRepository.GetById(foodItemId);
        if (foodItemEntity == null)
            return NotFound();
        string senderPhone = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        string? ownerPermissionCheckMsg =
            await authService.CheckOwnerPermission(senderPhone, foodItemEntity.RestaurantId);

        if (ownerPermissionCheckMsg != null)
            return BadRequest(ownerPermissionCheckMsg);
        if (foodItemEntity.CldnrPublicId != null)
        {
            var deleteResult = await imageService.DeleteImageAsync(foodItemEntity.CldnrPublicId);
            if(deleteResult.Error != null)
                return BadRequest(deleteResult.Error.Message);
            foodItemEntity.CldnrPublicId = foodItemEntity.CldnrUrl = null;
        }
        
        var uploadResult = await imageService.AddImageAsync(image, 300, 300);
        if (uploadResult.Error != null)
            return BadRequest(uploadResult.Error.Message);

        foodItemEntity.CldnrPublicId = uploadResult.PublicId;
        foodItemEntity.CldnrUrl = uploadResult.SecureUrl.AbsoluteUri;

        await restaurantRepository.SaveChangesAsync();
        return Ok(mapper.Map<FoodItemDto>(foodItemEntity));
    }
}
