using Microsoft.EntityFrameworkCore;
using Restaurant.Data;
using Restaurant.Domain.DTOs;
using Restaurant.Domain.Entities;
using Restaurant.Service.Interfaces;
using Dapper;
namespace Restaurant.Service.Services
{
    public class OrderService : IOrderService
    {
        private readonly RestaurantDbContext _context;

        public OrderService(RestaurantDbContext context)
        {
            _context = context;
        }












        public async Task<OrderDto?> GetLatestOrderDetailsByTableIdAsync(string tableId)
        {
            // Lấy session mới nhất (chưa đóng)
            var latestSession = await _context.TableSessions
                .Where(ts => ts.TableId == tableId && ts.CloseAt == null) // tương đương IS NULL
                .OrderByDescending(ts => ts.OpenAt)
                .FirstOrDefaultAsync();

            if (latestSession == null)
                return null;

            // Lấy order gắn với session 
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Dish)
                .Include(o => o.OrderTables)
                    .ThenInclude(ot => ot.Table)
                        .ThenInclude(t => t.Area)
                .FirstOrDefaultAsync(o => o.TableSessionId == latestSession.SessionId);

            if (order == null)
                return null;

            // Map sang DTO
            return new OrderDto
            {
                OrderId = order.OrderId,
                OrderDate = order.CreatedAt,
                IsPaid = order.IsPaid,
                TableCode = order.OrderTables.FirstOrDefault(ot => ot.IsPrimary)?.Table?.TableCode
                            ?? order.OrderTables.FirstOrDefault()?.Table?.TableCode ?? string.Empty,
                TableName = order.OrderTables.FirstOrDefault(ot => ot.IsPrimary)?.Table?.TableName
                            ?? order.OrderTables.FirstOrDefault()?.Table?.TableName,
                AreaName = order.OrderTables.FirstOrDefault(ot => ot.IsPrimary)?.Table?.Area?.AreaName
                            ?? order.OrderTables.FirstOrDefault()?.Table?.Area?.AreaName,
                TotalAmount = order.OrderDetails.Sum(od => od.TotalPrice),
                TableSessionId = order.TableSessionId,
                OrderDetails = order.OrderDetails.Select(od => new OrderDetailDto
                {
                    OrderDetailId = od.OrderDetailId,
                    DishId = od.DishId,
                    DishName = od.Dish?.DishName ?? string.Empty,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice
                }).ToList()
            };
        }



    }
}