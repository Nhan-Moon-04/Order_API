using Microsoft.AspNetCore.Mvc;
using Restaurant.Domain.DTOs;
using Restaurant.Service.Interfaces;
using Restaurant.Domain.DTOs.Request;

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

        [HttpPost("add-food")]
        public async Task<ActionResult<OrderDetailDto>> AddFood(OrderDetailDto orderDetailDto)
        {
            try
            {
                var createdOrderDetail = await _orderDetailService.AddFood(orderDetailDto);
                return CreatedAtAction(nameof(GetOrderDetail), new { id = createdOrderDetail.OrderDetailId }, createdOrderDetail);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }




        [HttpPost]
        public async Task<ActionResult<OrderDetailDto>> AddFoodToOrder([FromBody] AddFoodRequest request)
        {
            try
            {
                var result = await _orderDetailService.AddFoodToOrder(
                    request.OrderId,
                    request.DishId,
                    request.Quantity
                );

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPost("order/{orderId}/multiple-dishes")]
        public async Task<ActionResult<IEnumerable<OrderDetailDto>>> AddMultipleFoodsToOrder(string orderId, [FromBody] Dictionary<string, int> dishQuantities)
        {
            try
            {
                var results = await _orderDetailService.AddMultipleFoodsToOrder(orderId, dishQuantities);
                return Ok(results);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
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