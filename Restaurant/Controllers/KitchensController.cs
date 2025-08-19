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
    public class KitchensController : ControllerBase
    {
        private readonly IKitchensService _kitchen;

        public KitchensController(IKitchensService context)
        {
            _kitchen = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Kitchens>>> GetAllAsync()
        {
            var kitchens = await _kitchen.GetAllAsync();
            return Ok(kitchens);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Kitchens>> GetByIdAsync(string id)
        {
            var kitchen = await _kitchen.GetByIdAsync(id);
            if (kitchen == null)
            {
                return NotFound();
            }
            return Ok(kitchen);
        }

        [HttpPost]
        public async Task<ActionResult<Kitchens>> CreateAsync([FromBody] KitchensDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Kitchen data is required.");
            }
            var created = await _kitchen.CreateAsync(dto);
            return CreatedAtAction(nameof(GetByIdAsync), new { id = created.KitchenId }, created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Kitchens>> UpdateAsync(string id, [FromBody] KitchensDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Kitchen data is required.");
            }
            var updated = await _kitchen.UpdateAsync(id, dto);
            if (updated == null)
            {
                return NotFound();
            }
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            var success = await _kitchen.DeleteAsync(id);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();

        }
    }
}
