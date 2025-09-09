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

        [HttpPost ("AddFood")]
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

        //[HttpPost("order/{orderId}/multiple-dishes")]
        //public async Task<ActionResult<IEnumerable<OrderDetailDto>>> AddMultipleFoodsToOrder(string orderId, [FromBody] Dictionary<string, int> dishQuantities)
        //{
        //    try
        //    {
        //        var results = await _orderDetailService.AddMultipleFoodsToOrder(orderId, dishQuantities);
        //        return Ok(results);
        //    }
        //    catch (ArgumentException ex)
        //    {
        //        return BadRequest(new { message = ex.Message });
        //    }
        //}

        /// <summary>
        /// Xóa hoàn toàn món ăn khỏi order (không quan tâm số lượng)
        /// </summary>
        [HttpPost("remove-food")]
        public async Task<ActionResult> RemoveFood(OrderDetailDto orderDetailDto)
        {
            try
            {
                var result = await _orderDetailService.RemoveFood(orderDetailDto);
                return Ok(new { message = "Món ăn đã được xóa hoàn toàn khỏi order", success = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Xóa hoàn toàn món ăn khỏi order (không quan tâm số lượng)
        /// </summary>
        [HttpPost("RemoveFood")]
        public async Task<ActionResult> RemoveFoodFromOrder([FromBody] RemoveFoodRequest request)
        {
            try
            {
                var result = await _orderDetailService.RemoveFoodFromOrder(
                    request.OrderId,
                    request.DishId
                );

                return Ok(new { message = "Món ăn đã được xóa hoàn toàn khỏi order", success = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("change-quantity")]
        public async Task<ActionResult<OrderDetailDto>> ChangeQuantityFood([FromBody] ChangeQuantityRequest request)
        {
            try
            {
                var result = await _orderDetailService.ChangeQuantityFood(
                    request.OrderId,
                    request.DishId,
                    request.NewQuantity
                );

                if (result == null)
                {
                    return Ok(new { message = "Món ăn đã được xóa khỏi order do số lượng = 0", isRemoved = true });
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