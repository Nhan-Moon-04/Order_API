using Microsoft.AspNetCore.Mvc;
using Restaurant.Domain.DTOs;
using Restaurant.Domain.DTOs.Request;
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

        [HttpPost("remove-food")]
        public async Task<ActionResult<OrderDetailDto>> RemoveFood(OrderDetailDto orderDetailDto)
        {
            try
            {
                var result = await _orderDetailService.RemoveFood(orderDetailDto);
                if (result == null)
                {
                    return Ok(new { message = "Món ăn đã được xóa hoàn toàn khỏi order", isCompletelyRemoved = true });
                }
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("order/{orderId}/dish/{dishId}/remove")]
        public async Task<ActionResult<OrderDetailDto>> RemoveFoodFromOrder(string orderId, string dishId, [FromQuery] int quantity = 1)
        {
            try
            {
                var result = await _orderDetailService.RemoveFoodFromOrder(orderId, dishId, quantity);
                if (result == null)
                {
                    return Ok(new { message = "Món ăn đã được xóa hoàn toàn khỏi order", isCompletelyRemoved = true });
                }
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("order/{orderId}/remove-multiple-dishes")]
        public async Task<ActionResult<IEnumerable<OrderDetailDto?>>> RemoveMultipleFoodsFromOrder(string orderId, [FromBody] Dictionary<string, int> dishQuantities)
        {
            try
            {
                var results = await _orderDetailService.RemoveMultipleFoodsFromOrder(orderId, dishQuantities);
                return Ok(results);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("remove-food-v2")]
        public async Task<ActionResult<OrderDetailDto>> RemoveFoodV2([FromBody] RemoveFoodRequest request)
        {
            try
            {
                var dto = new OrderDetailDto
                {
                    OrderId = request.OrderId,
                    DishId = request.DishId,
                    Quantity = request.Quantity
                };

                var result = await _orderDetailService.RemoveFood(dto);
                if (result == null)
                {
                    return Ok(new { 
                        message = "Món ăn đã được xóa hoàn toàn khỏi order", 
                        isCompletelyRemoved = true,
                        reason = request.Reason ?? "Không có lý do"
                    });
                }
                return Ok(new { orderDetail = result, reason = request.Reason });
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