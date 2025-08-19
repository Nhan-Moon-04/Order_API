using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restaurant.Data;
using Restaurant.Domain.Entities;

namespace Restaurant.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DishesController : ControllerBase
    {
        private readonly RestaurantDbContext _context;

        public DishesController(RestaurantDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Dishes>>> GetDishes()
        {
            return await _context.Dishes
                                 .Include(d => d.Kitchen)
                                 .Include(d => d.AreaDishPrices)
                                 .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Dishes>> GetDish(string id)
        {
            var dish = await _context.Dishes
                                     .Include(d => d.Kitchen)
                                     .Include(d => d.AreaDishPrices)
                                     .FirstOrDefaultAsync(d => d.DishId == id);

            if (dish == null) return NotFound();
            return dish;
        }

        [HttpPost]
        public async Task<ActionResult<Dishes>> CreateDish(Dishes dish)
        {
            _context.Dishes.Add(dish);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetDish), new { id = dish.DishId }, dish);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDish(string id, Dishes dish)
        {
            if (id != dish.DishId) return BadRequest();
            _context.Entry(dish).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDish(string id)
        {
            var dish = await _context.Dishes.FindAsync(id);
            if (dish == null) return NotFound();
            _context.Dishes.Remove(dish);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
