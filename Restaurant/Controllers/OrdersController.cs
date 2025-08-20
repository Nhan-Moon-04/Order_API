using Microsoft.AspNetCore.Mvc;
using Restaurant.Domain.DTOs;
using Restaurant.Service.Interfaces;

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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrder(string id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }

        [HttpGet("table/{tableCode}")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersByTable(string tableCode)
        {
            var orders = await _orderService.GetOrdersByTableIdAsync(tableCode);
            return Ok(orders);
        }

        [HttpGet("unpaid")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetUnpaidOrders()
        {
            var orders = await _orderService.GetUnpaidOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("date-range")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersByDateRange(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            var orders = await _orderService.GetOrdersByDateRangeAsync(startDate, endDate);
            return Ok(orders);
        }

        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder(OrderDto orderDto)
        {
            var createdOrder = await _orderService.CreateOrderAsync(orderDto);
            return CreatedAtAction(nameof(GetOrder), new { id = createdOrder.OrderId }, createdOrder);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(string id, OrderDto orderDto)
        {
            var updatedOrder = await _orderService.UpdateOrderAsync(id, orderDto);
            if (updatedOrder == null)
            {
                return NotFound();
            }
            return Ok(updatedOrder);
        }

        [HttpPut("{id}/pay")]
        public async Task<IActionResult> MarkOrderAsPaid(string id)
        {
            var result = await _orderService.MarkOrderAsPaidAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return Ok(new { Message = "Order marked as paid successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(string id)
        {
            var result = await _orderService.DeleteOrderAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}