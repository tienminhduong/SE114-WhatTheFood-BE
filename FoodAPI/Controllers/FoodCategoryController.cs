using AutoMapper;
using FoodAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FoodAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "UserAccessLevel")]
    public class FoodCategoryController(IFoodCategoryRepository foodCategoryRepository,
        IMapper mapper) : ControllerBase
    {
        
    }
}
