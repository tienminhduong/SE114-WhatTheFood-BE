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
    IUserRepository userRepository,
    IMapper mapper
    ) : ControllerBase
{
    [Authorize(Policy = "UserAccessLevel")]
    [HttpPost("profile")]
    public async Task<ActionResult<UserDto>> UploadUserProfileImage(IFormFile file)
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

        var uploadResult = await imageService.AddImageAsync(file, 300, 300);
        if (uploadResult.Error != null)
            return BadRequest(uploadResult.Error.Message);

        user.PfpPublicId = uploadResult.PublicId;
        user.PfpUrl = uploadResult.SecureUrl.AbsoluteUri;

        await userRepository.SaveChangesAsync();
        return Ok(mapper.Map<UserDto>(user));
    }
}
