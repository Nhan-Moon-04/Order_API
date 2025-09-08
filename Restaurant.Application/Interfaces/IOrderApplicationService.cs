using Restaurant.Application.DTOs;

namespace Restaurant.Application.Interfaces
{
    public interface IOrderApplicationService
    {
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
        Task<OrderDto?> GetOrderByIdAsync(string id);
        Task<IEnumerable<OrderDto>> GetOrdersByTableIdAsync(string tableCode);
        Task<IEnumerable<OrderDto>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<OrderDto> CreateOrderAsync(OrderDto dto);
        Task<OrderDto?> UpdateOrderAsync(string id, OrderDto dto);
        Task<bool> DeleteOrderAsync(string id);
        Task<bool> MarkOrderAsPaidAsync(string id);
        Task<IEnumerable<OrderDto>> GetUnpaidOrdersAsync();
        Task<IEnumerable<OrderDto>> GetOrdersByTableSessionIdAsync(string tableSessionId);
        Task<OrderDto?> GetLatestOrderDetailsByTableIdAsync(string tableId);
    }
}