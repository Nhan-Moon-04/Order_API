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
    public class DishGroupController : ControllerBase // Changed from implementing IDishesGroupService
    {
        public readonly IDishesGroupService _service;

        public DishGroupController(IDishesGroupService service) // Fixed constructor parameter type
        {
            _service = service;
        }

        // GET: api/dishgroup
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DishGroupDto>>> GetAllGroup()
        {
            var groups = await _service.GetAllGroup();
            return Ok(groups);
        }
    }
}