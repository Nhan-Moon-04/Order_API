using Restaurant.Domain.Entities;

namespace Restaurant.Application.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllAsync();
        Task<Order?> GetByIdAsync(string id);
        Task<IEnumerable<Order>> GetByTableIdAsync(string tableCode);
        Task<IEnumerable<Order>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<Order> CreateAsync(Order order);
        Task<Order?> UpdateAsync(string id, Order order);
        Task<bool> DeleteAsync(string id);
        Task<IEnumerable<Order>> GetUnpaidOrdersAsync();
        Task<IEnumerable<Order>> GetByTableSessionIdAsync(string tableSessionId);
    }
}