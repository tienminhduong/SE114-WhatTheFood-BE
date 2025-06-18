using AutoMapper;
using FoodAPI.Entities;
using FoodAPI.Interfaces;
using FoodAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FoodAPI.Controllers
{
    [Route("api/foodcategories")]
    [ApiController]
    [Authorize(Policy = "UserAccessLevel")]
    public class FoodCategoryController(IFoodCategoryRepository foodCategoryRepository,
        IMapper mapper) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FoodCategoryDto>>> GetAllCategories()
        {
            var categories = await foodCategoryRepository.GetCategoriesAsync();
            return Ok(mapper.Map<IEnumerable<FoodCategoryDto>>(categories));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FoodCategory>> GetById(int id)
        {
            var category = await foodCategoryRepository.GetCategoryAsync(id);
            return Ok(mapper.Map<FoodCategoryDto>(category));
        }
    }
}
