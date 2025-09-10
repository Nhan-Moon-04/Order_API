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

    

     
    }
}