using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restaurant.Data;
using Restaurant.Domain.DTOs;
using Restaurant.Domain.Entities;
using Restaurant.Service.Interfaces;

namespace Restaurant.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DishesController : ControllerBase
    {
        public readonly IDishesService _service;
        public DishesController(IDishesService service)
        {
            _service = service;
        }

        // GET: api/dishes
        [HttpGet]

        public async Task<ActionResult<IEnumerable<DishesDto>>> GetAll()
        {
            var dishes = await _service.GetAllDishesAsync();
            return Ok(dishes);
        }

        // GET: api/dishes/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<DishesDto>> GetById(string id)
        {
            var dish = await _service.GetDishByIdAsync(id);
            if (dish == null) return NotFound();
            return Ok(dish);
        }

        // POST: api/dishes
        [HttpPost]
        public async Task<ActionResult<DishesDto>> CreateDishAsync([FromBody] DishesDto dishDto)
        {
            if (dishDto == null) return BadRequest("Dish data is required.");
            var createdDish = await _service.CreateDishAsync(dishDto);
            return CreatedAtAction(nameof(GetById), new { id = createdDish.DishId }, createdDish);
        }
    }
}