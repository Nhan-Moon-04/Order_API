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



        [HttpPost("SearchEF")]

        public async Task<ActionResult> GetPagedAreaDishPriceEFAsync(
            [FromBody] AreaDishPriceQueryParameters query)
        {
            try
            {
                var result = await _service.GetPagedAreaDishPriceAsyncEF(query);
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
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddAreaDishPrices([FromBody] AddAreaDishPriceRequest request)
        {
            if (request.DishIds == null || request.DishIds.Count == 0)
                return BadRequest("Phải chọn ít nhất 1 món");

            await _service.AddDishesToAreaAsync(request);
            return Ok(new { Message = "Đã thêm món vào khu thành công" });
        }

        [HttpPost("DeleteAreaDishPrice")]
        public async Task<IActionResult> DeleteAreaDishPrice([FromBody] DeleteAreaDishPrice request)
        {
            try
            {
                var result = await _service.DeleteAsync(request.Id);
                if (result)
                    return Ok(new { Message = "Xoá món khỏi khu vực thành công" });
                else
                    return NotFound(new { Message = "Không tìm thấy mục để xoá" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("GetDishNames")]
        public async Task<IActionResult> GetDishNames([FromBody] GetDishNamesRequest request)
        {
            try
            {
                var dishNames = await _service.GetDishNames(request.SearchString, request.SearchName);
                return Ok(new
                {
                    statusCode = 200,
                    isSuccess = true,
                    message = "Dish names retrieved successfully.",
                    data = dishNames
                });
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
