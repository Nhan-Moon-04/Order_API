using Microsoft.EntityFrameworkCore;
using Restaurant.Data;
using Restaurant.Domain.DTOs;
using Restaurant.Domain.Entities;
using Restaurant.Service.Interfaces;

namespace Restaurant.Service.Services
{
    public class OrderService : IOrderService
    {
        private readonly RestaurantDbContext _context;

        public OrderService(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.Table)
                    .ThenInclude(t => t!.Area)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Dish)
                .ToListAsync();

            return orders.Select(order => new OrderDto
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                IsPaid = order.IsPaid,
                TableCode = order.TableCode,
                TableName = order.Table?.TableName,
                AreaName = order.Table?.Area?.AreaName,
                TotalAmount = order.OrderDetails.Sum(od => od.TotalPrice),
                OrderDetails = order.OrderDetails.Select(od => new OrderDetailDto
                {
                    OrderDetailId = od.OrderDetailId,
                    OrderId = od.OrderId,
                    DishId = od.DishId,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice,
                    DishName = od.Dish?.DishName
                }).ToList()
            });
        }

        public async Task<OrderDto?> GetOrderByIdAsync(string id)
        {
            var order = await _context.Orders
                .Include(o => o.Table)
                    .ThenInclude(t => t!.Area)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Dish)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            return order == null ? null : new OrderDto
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                IsPaid = order.IsPaid,
                TableCode = order.TableCode,
                TableName = order.Table?.TableName,
                AreaName = order.Table?.Area?.AreaName,
                TotalAmount = order.OrderDetails.Sum(od => od.TotalPrice),
                OrderDetails = order.OrderDetails.Select(od => new OrderDetailDto
                {
                    OrderDetailId = od.OrderDetailId,
                    OrderId = od.OrderId,
                    DishId = od.DishId,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice,
                    DishName = od.Dish?.DishName
                }).ToList()
            };
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByTableIdAsync(string tableCode)
        {
            var orders = await _context.Orders
                .Include(o => o.Table)
                    .ThenInclude(t => t!.Area)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Dish)
                .Where(o => o.TableCode == tableCode)
                .ToListAsync();

            return orders.Select(order => new OrderDto
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                IsPaid = order.IsPaid,
                TableCode = order.TableCode,
                TableName = order.Table?.TableName,
                AreaName = order.Table?.Area?.AreaName,
                TotalAmount = order.OrderDetails.Sum(od => od.TotalPrice)
            });
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var orders = await _context.Orders
                .Include(o => o.Table)
                    .ThenInclude(t => t!.Area)
                .Include(o => o.OrderDetails)
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .ToListAsync();

            return orders.Select(order => new OrderDto
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                IsPaid = order.IsPaid,
                TableCode = order.TableCode,
                TableName = order.Table?.TableName,
                AreaName = order.Table?.Area?.AreaName,
                TotalAmount = order.OrderDetails.Sum(od => od.TotalPrice)
            });
        }

        public async Task<OrderDto> CreateOrderAsync(OrderDto dto)
        {
            var entity = new Order
            {
                OrderId = dto.OrderId,
                OrderDate = dto.OrderDate,
                IsPaid = dto.IsPaid,
                TableCode = dto.TableCode
            };

            _context.Orders.Add(entity);
            await _context.SaveChangesAsync();

            return dto;
        }

        public async Task<OrderDto?> UpdateOrderAsync(string id, OrderDto dto)
        {
            var entity = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == id);
            if (entity == null) return null;

            entity.OrderDate = dto.OrderDate;
            entity.IsPaid = dto.IsPaid;
            entity.TableCode = dto.TableCode;

            await _context.SaveChangesAsync();
            return dto;
        }

        public async Task<bool> DeleteOrderAsync(string id)
        {
            var entity = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.OrderId == id);
            if (entity == null) return false;

            _context.Orders.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkOrderAsPaidAsync(string id)
        {
            var entity = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == id);
            if (entity == null) return false;

            entity.IsPaid = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<OrderDto>> GetUnpaidOrdersAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.Table)
                    .ThenInclude(t => t!.Area)
                .Include(o => o.OrderDetails)
                .Where(o => !o.IsPaid)
                .ToListAsync();

            return orders.Select(order => new OrderDto
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                IsPaid = order.IsPaid,
                TableCode = order.TableCode,
                TableName = order.Table?.TableName,
                AreaName = order.Table?.Area?.AreaName,
                TotalAmount = order.OrderDetails.Sum(od => od.TotalPrice)
            });
        }
    }
}