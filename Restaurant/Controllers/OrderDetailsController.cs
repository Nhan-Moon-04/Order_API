using Microsoft.AspNetCore.Mvc;
using Restaurant.Domain.DTOs;
using Restaurant.Service.Interfaces;

namespace Restaurant.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderDetailsController : ControllerBase
    {
        private readonly IOrderDetailService _orderDetailService;

        public OrderDetailsController(IOrderDetailService orderDetailService)
        {
            _orderDetailService = orderDetailService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDetailDto>>> GetAllOrderDetails()
        {
            var orderDetails = await _orderDetailService.GetAllOrderDetailsAsync();
            return Ok(orderDetails);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDetailDto>> GetOrderDetail(string id)
        {
            var orderDetail = await _orderDetailService.GetOrderDetailByIdAsync(id);
            if (orderDetail == null)
            {
                return NotFound();
            }
            return Ok(orderDetail);
        }

        [HttpGet("order/{orderId}")]
        public async Task<ActionResult<IEnumerable<OrderDetailDto>>> GetOrderDetailsByOrder(string orderId)
        {
            var orderDetails = await _orderDetailService.GetOrderDetailsByOrderIdAsync(orderId);
            return Ok(orderDetails);
        }

        [HttpGet("order/{orderId}/total")]
        public async Task<ActionResult<double>> GetOrderTotal(string orderId)
        {
            var total = await _orderDetailService.GetOrderTotalAsync(orderId);
            return Ok(total);
        }

        [HttpPost]
        public async Task<ActionResult<OrderDetailDto>> CreateOrderDetail(OrderDetailDto orderDetailDto)
        {
            var createdOrderDetail = await _orderDetailService.CreateOrderDetailAsync(orderDetailDto);
            return CreatedAtAction(nameof(GetOrderDetail), new { id = createdOrderDetail.OrderDetailId }, createdOrderDetail);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrderDetail(string id, OrderDetailDto orderDetailDto)
        {
            var updatedOrderDetail = await _orderDetailService.UpdateOrderDetailAsync(id, orderDetailDto);
            if (updatedOrderDetail == null)
            {
                return NotFound();
            }
            return Ok(updatedOrderDetail);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderDetail(string id)
        {
            var result = await _orderDetailService.DeleteOrderDetailAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}