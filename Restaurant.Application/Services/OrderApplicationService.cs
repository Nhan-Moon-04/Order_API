using Restaurant.Application.DTOs;
using Restaurant.Application.Interfaces;
using Restaurant.Application.Interfaces.Repositories;
using Restaurant.Domain.Entities;

namespace Restaurant.Application.Services
{
    public class OrderApplicationService : IOrderApplicationService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderApplicationService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllAsync();
            return orders.Select(MapToDto);
        }

        public async Task<OrderDto?> GetOrderByIdAsync(string id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            return order != null ? MapToDto(order) : null;
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByTableIdAsync(string tableCode)
        {
            var orders = await _orderRepository.GetByTableIdAsync(tableCode);
            return orders.Select(MapToDto);
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var orders = await _orderRepository.GetByDateRangeAsync(startDate, endDate);
            return orders.Select(MapToDto);
        }

        public async Task<OrderDto> CreateOrderAsync(OrderDto dto)
        {
            var order = MapToEntity(dto);
            var createdOrder = await _orderRepository.CreateAsync(order);
            return MapToDto(createdOrder);
        }

        public async Task<OrderDto?> UpdateOrderAsync(string id, OrderDto dto)
        {
            var order = MapToEntity(dto);
            var updatedOrder = await _orderRepository.UpdateAsync(id, order);
            return updatedOrder != null ? MapToDto(updatedOrder) : null;
        }

        public async Task<bool> DeleteOrderAsync(string id)
        {
            return await _orderRepository.DeleteAsync(id);
        }

        public async Task<bool> MarkOrderAsPaidAsync(string id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null) return false;

            order.IsPaid = true;
            var updated = await _orderRepository.UpdateAsync(id, order);
            return updated != null;
        }

        public async Task<IEnumerable<OrderDto>> GetUnpaidOrdersAsync()
        {
            var orders = await _orderRepository.GetUnpaidOrdersAsync();
            return orders.Select(MapToDto);
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByTableSessionIdAsync(string tableSessionId)
        {
            var orders = await _orderRepository.GetByTableSessionIdAsync(tableSessionId);
            return orders.Select(MapToDto);
        }

        public async Task<OrderDto?> GetLatestOrderDetailsByTableIdAsync(string tableId)
        {
            var orders = await _orderRepository.GetByTableIdAsync(tableId);
            var latestOrder = orders.OrderByDescending(o => o.CreatedAt).FirstOrDefault();
            return latestOrder != null ? MapToDto(latestOrder) : null;
        }

        private OrderDto MapToDto(Order order)
        {
            return new OrderDto
            {
                OrderId = order.OrderId,
                OrderDate = order.CreatedAt,
                IsPaid = order.IsPaid,
                CreatedAt = order.CreatedAt,
                ClosedAt = order.ClosedAt,
                TableSessionId = order.TableSessionId,
                TotalAmount = (decimal)order.TotalAmount
            };
        }

        private Order MapToEntity(OrderDto dto)
        {
            return new Order
            {
                OrderId = dto.OrderId,
                CreatedAt = dto.CreatedAt,
                IsPaid = dto.IsPaid,
                ClosedAt = dto.ClosedAt,
                TableSessionId = dto.TableSessionId,
                TotalAmount = (double)dto.TotalAmount
            };
        }
    }
}