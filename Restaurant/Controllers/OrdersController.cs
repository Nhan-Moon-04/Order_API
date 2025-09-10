using Microsoft.AspNetCore.Mvc;
using Restaurant.Domain.DTOs;
using Restaurant.Service.Interfaces;
using Restaurant.Service.Services;

namespace Restaurant.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }






        [HttpGet("{tableId}/latest-order")]
        public async Task<ActionResult<OrderDto>> GetLatestOrder(string tableId)
        {
            var order = await _orderService.GetLatestOrderDetailsByTableIdAsync(tableId);
            if (order == null)
                return NotFound();
            return Ok(order);
        }

      
        // New endpoint to get active order for a table (based on active session)
        [HttpGet("table/{tableId}/active")]
        public async Task<ActionResult<OrderDto>> GetActiveOrderByTableId(string tableId)
        {
            var order = await _orderService.GetLatestOrderDetailsByTableIdAsync(tableId);
            if (order == null)
                return NotFound(new { Message = "Không có order đang hoạt động cho bàn này" });
            return Ok(order);
        }

     
    }
}