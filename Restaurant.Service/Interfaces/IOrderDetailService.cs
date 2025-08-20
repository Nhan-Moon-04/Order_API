using Restaurant.Domain.DTOs;

namespace Restaurant.Service.Interfaces
{
    public interface IOrderDetailService
    {
        Task<IEnumerable<OrderDetailDto>> GetAllOrderDetailsAsync();
        Task<OrderDetailDto?> GetOrderDetailByIdAsync(string id);
        Task<IEnumerable<OrderDetailDto>> GetOrderDetailsByOrderIdAsync(string orderId);
        Task<OrderDetailDto> CreateOrderDetailAsync(OrderDetailDto dto);
        Task<OrderDetailDto?> UpdateOrderDetailAsync(string id, OrderDetailDto dto);
        Task<bool> DeleteOrderDetailAsync(string id);
        Task<double> GetOrderTotalAsync(string orderId);
    }
}