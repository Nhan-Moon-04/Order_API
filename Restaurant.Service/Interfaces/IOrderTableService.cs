using Restaurant.Domain.DTOs;

namespace Restaurant.Service.Interfaces
{
    public interface IOrderTableService
    {
        Task<IEnumerable<OrderTableDto>> GetAllAsync();
        Task<OrderTableDto?> GetByIdAsync(string id);
        Task<IEnumerable<OrderTableDto>> GetByOrderIdAsync(string orderId);
        Task<IEnumerable<OrderTableDto>> GetByTableIdAsync(string tableId);
        Task<OrderTableDto> CreateAsync(OrderTableDto orderTableDto);
        Task<OrderTableDto?> UpdateAsync(string id, OrderTableDto orderTableDto);
        Task<bool> DeleteAsync(string id);
        Task<OrderTableDto?> GetPrimaryTableByOrderIdAsync(string orderId);
        Task<bool> SetPrimaryTableAsync(string orderId, string tableId);
    }
}