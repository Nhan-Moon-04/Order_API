using Restaurant.Domain.DTOs;

namespace Restaurant.Service.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
        Task<OrderDto?> GetOrderByIdAsync(string id);
        Task<IEnumerable<OrderDto>> GetOrdersByTableIdAsync(string tableCode);
        Task<OrderDto?> GetLatestOrderDetailsByTableIdAsync(string tableId);
    }
}