using AutoMapper;
using FoodAPI.Entities;
using FoodAPI.Interfaces;
using FoodAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace FoodAPI.Controllers;

[Route("api/users")]
[ApiController]
public class UserController(IUserRepository userRepository,
    IMapper mapper): ControllerBase
{
    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        var userEntities = await _userRepository.GetUsersAsync();
        return Ok(_mapper.Map<IEnumerable<UserDto>>(userEntities));
    }
    
    [HttpGet("{userid}", Name = "GetUserById")]
    public async Task<ActionResult<User>> GetUser(int userid)
    {
        var userEntity = await _userRepository.GetUserAsync(userid);
        if (userEntity == null)
            return NotFound();
        return Ok(_mapper.Map<UserDto>(userEntity));
    }

    [HttpPost]
    public async Task<ActionResult<User>> CreateUserAsync(UserForCreationDto user)
    {
        var userEntity = _mapper.Map<Entities.User>(user);
        await _userRepository.AddUserAsync(userEntity);
        await _userRepository.SaveChangesAsync();
        var createdUser = _mapper.Map<UserDto>(userEntity);
        return CreatedAtRoute("GetUserById",
            new { userid = userEntity.Id },
            createdUser
            );
    }
}