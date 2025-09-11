using Microsoft.AspNetCore.Mvc;
using Restaurant.Domain.DTOs;
using Restaurant.Domain.DTOs.Query;
using Restaurant.Domain.DTOs.Request;
using Restaurant.Service.Interfaces;
using Restaurant.Service.Services;

namespace Restaurant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AreaDishPricesController : ControllerBase
    {
        private readonly IAreaDishPriceService _service;

        public AreaDishPricesController(IAreaDishPriceService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AreaDishPriceDto>>> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpPost("Prices")]
        public async Task<ActionResult<IEnumerable<AreaDishPriceDto>>> Get([FromBody] GetAreaDishPrice request)
        {
            if (string.IsNullOrWhiteSpace(request.Area))
                return BadRequest("Id is required.");

            var tables = await _service.GetByIdAsync(request.Area);
            return Ok(tables);
        }







        [HttpPost("update-price")]
        public async Task<ActionResult<AreaDishPriceDto>> UpdatePrice([FromBody] UpdatePriceRequest request)
        {
            try
            {
                var result = await _service.UpdatePriceAsync(request.Id, request.CustomPrice);
                return Ok(new { message = "Cập nhật giá thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("Search")]
        public async Task<ActionResult> GetPagedAreaDishPriceAsync(
            [FromBody] AreaDishPriceQueryParameters query)
        {
            var result = await _service.GetPagedAreaDishPriceAsync(query);

            var data = result.Item1;
            var totalRecords = result.Item2;

            return Ok(new
            {
                Items = data,
                TotalRecords = totalRecords,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)query.PageSize)
            });
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddAreaDishPrices([FromBody] AddAreaDishPriceRequest request)
        {
            if (request.DishIds == null || request.DishIds.Count == 0)
                return BadRequest("Phải chọn ít nhất 1 món");

            await _service.AddDishesToAreaAsync(request);
            return Ok(new { Message = "Đã thêm món vào khu thành công" });
        }



    }
}
