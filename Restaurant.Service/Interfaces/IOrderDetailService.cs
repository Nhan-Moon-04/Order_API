using Restaurant.Domain.DTOs;

namespace Restaurant.Service.Interfaces
{
    public interface IOrderDetailService
    {
        Task<IEnumerable<OrderDetailDto>> GetAllOrderDetailsAsync();
        Task<IEnumerable<OrderDetailDto>> GetOrderDetailsByOrderIdAsync(string orderId);
        Task<OrderDetailDto> AddFood(OrderDetailDto dto);
        Task<OrderDetailDto> AddFoodToOrder(string orderId, string dishId, int quantity = 1);
        
        // Xóa hoàn toàn món ăn khỏi order
        Task<bool> RemoveFood(OrderDetailDto dto);

        
        // Thay đổi số lượng món ăn
        Task<OrderDetailDto?> ChangeQuantityFood(string orderId, string dishId, int newQuantity);
    }
}