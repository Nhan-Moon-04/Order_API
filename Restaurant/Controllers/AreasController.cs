using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restaurant.Data;
using Restaurant.Domain.DTOs;
using Restaurant.Domain.Entities;
using Restaurant.Service.Interfaces;

namespace Restaurant.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AreasController : ControllerBase
    {
        private readonly IAreasService _service;

        public AreasController(IAreasService service)
        {
            _service = service;
        }

        // GET: api/areas
        [HttpGet]
        public async Task<IActionResult> GetAreas()
        {
            var areas = await _service.GetAllAsync();
            return Ok(areas);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<AreasDto>> Get(string id)
        {
            var dto = await _service.GetByIdAsync(id);
            return dto == null ? NotFound() : Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<AreasDto>> Create([FromBody] AreasDto dto)
        {
            if (dto == null) return BadRequest("Area data is required.");
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = created.AreaId }, created);
        }   

        [HttpGet("count")]
        public async Task<ActionResult<int>> CountAreas()
        {
            var count = await _service.CountAresa();
            return Ok(count);
        }
    }
}
