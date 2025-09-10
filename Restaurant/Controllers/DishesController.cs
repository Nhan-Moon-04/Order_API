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

   
    }
}