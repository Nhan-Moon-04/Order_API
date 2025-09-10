using Restaurant.Domain.DTOs;

namespace Restaurant.Service.Interfaces
{
    public interface IOrderService
    {

        Task<OrderDto?> GetLatestOrderDetailsByTableIdAsync(string tableId);
    }
}