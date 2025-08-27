using Restaurant.Domain.DTOs;

namespace Restaurant.Service.Interfaces
{
    public interface IOrderDetailService
    {
        Task<IEnumerable<OrderDetailDto>> GetAllOrderDetailsAsync();
        Task<OrderDetailDto?> GetOrderDetailByIdAsync(string id);
        Task<IEnumerable<OrderDetailDto>> GetOrderDetailsByOrderIdAsync(string orderId);
        Task<OrderDetailDto> AddFood(OrderDetailDto dto);
        Task<OrderDetailDto?> UpdateOrderDetailAsync(string id, OrderDetailDto dto);
        Task<bool> DeleteOrderDetailAsync(string id);
        Task<double> GetOrderTotalAsync(string orderId);
        Task<OrderDetailDto> AddFoodToOrder(string orderId, string dishId, int quantity = 1);
        Task<IEnumerable<OrderDetailDto>> AddMultipleFoodsToOrder(string orderId, Dictionary<string, int> dishQuantities);
        
        // Xóa hoàn toàn món ăn khỏi order
        Task<bool> RemoveFood(OrderDetailDto dto);
        Task<bool> RemoveFoodFromOrder(string orderId, string dishId);
        Task<IEnumerable<OrderDetailDto?>> RemoveMultipleFoodsFromOrder(string orderId, Dictionary<string, int> dishQuantities);
        
        // Thay đổi số lượng món ăn
        Task<OrderDetailDto?> ChangeQuantityFood(string orderId, string dishId, int newQuantity);
    }
}