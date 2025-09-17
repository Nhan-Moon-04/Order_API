using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restaurant.Data;
using Restaurant.Domain.DTOs;
using Restaurant.Domain.Entities;
using Restaurant.Service.Interfaces;
using Restaurant.Domain.DTOs.Request;


using static Restaurant.Service.Services.AreasService;
namespace Restaurant.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AreasController : ControllerBase
    {
        private readonly IAreasService _service;
        private readonly AreasDapperService areasDapperService;
        public AreasController(IAreasService service, AreasDapperService dapper)
        {
            _service = service;
            areasDapperService = dapper;
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



        [HttpGet("count")]
        public async Task<ActionResult<int>> CountAreas()
        {
            var count = await _service.CountAresa();
            return Ok(count);
        }


        [HttpPost("update-active")]
        public async Task<IActionResult> UpdateActiveStatus([FromBody] UpdateIsActiveRrequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Id))
                return BadRequest("Id is required.");
            try
            {
                var updatedArea = await areasDapperService.UpdateIsActivate(request.Id, request.IsActive);
                if (updatedArea == null)
                    return NotFound($"No area found with Id: {request.Id}");
                return Ok(updatedArea);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



    }
}
