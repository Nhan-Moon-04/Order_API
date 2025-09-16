using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restaurant.Data;
using Restaurant.Domain.DTOs;
using Restaurant.Domain.Entities;
using Restaurant.Service.Interfaces;
using Restaurant.Service.Services;
using static Restaurant.Service.Services.DishesService;
using Restaurant.Domain.DTOs.Query;
using Restaurant.Domain.DTOs.Request;

namespace Restaurant.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DishesController : ControllerBase
    {
        public readonly DishesServiceDapper _services;
        public readonly IDishesService _service;
        public DishesController(IDishesService service, DishesServiceDapper services)
        {
            _service = service;
            _services = services;
        }

        // GET: api/dishes
        [HttpGet]

        public async Task<ActionResult<IEnumerable<DishesDto>>> GetAll()
        {
            var dishes = await _service.GetAllDishesAsync();
            return Ok(dishes);
        }

        [HttpPost("GetAllDishes")]
        public async Task<IActionResult> GetAllDishes([FromBody] DishesQueryParameters query)
        {
            var (data, totalRecords) = await _services.GetPagedDishesAsync(query);

            return Ok(new
            {
                Items = data,
                TotalRecords = totalRecords,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)query.PageSize)
            });
        }
        [HttpPost("AddDish")]
        public async Task<IActionResult> AddDish([FromBody] AddDishesRequest dish)
        {
            try
            {
                string shortGuid = Guid.NewGuid().ToString("N").Substring(0, 8);
                string newDishId = $"DISH{shortGuid}";
                var newDish = new DishesDto
                {
                    DishId = newDishId,
                    DishName = dish.DishName,
                    BasePrice = dish.BasePrice,
                    KitchenId = dish.KitchenId,
                    GroupId = dish.GroupId,
                    IsActive = dish.IsActive,
                    Description = dish.Description,
                    Id = Guid.NewGuid().ToString()
                };

                var result = await _services.AddDishAsync(newDish);

                if (result != null)
                {
                    return Ok(new
                    {
                        statusCode = 200,
                        isSuccess = true,
                        message = "Dish added successfully.",
                        data = result
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        statusCode = 400,
                        isSuccess = false,
                        message = "Failed to add dish."
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    isSuccess = false,
                    message = "An unexpected error occurred.",
                    errors = new[] { ex.Message }
                });
            }
        }


        [HttpPost("GetAvailableDishes")]
        public async Task<IActionResult> GetAvailableDishes([FromBody] GetAvailableDishesForAreaAsyncQuery request)
        {
            try
            {
                var dishes = await _services.GetAvailableDishesForAreaAsync(request);
                return Ok(StatusCode(200, new
                {
                    statusCode = 200,
                    isSuccess = true,
                    message = "Available dishes retrieved successfully.",
                    data = dishes
                }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    isSuccess = false,
                    message = "An unexpected error occurred.",
                    errors = new[] { ex.Message }
                });
            }
        }
    }
}