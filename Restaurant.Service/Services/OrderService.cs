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
                .Include(o => o.OrderTables)
                    .ThenInclude(ot => ot.Table)
                        .ThenInclude(t => t!.Area)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Dish)
                .Include(o => o.TableSession) // Include TableSession
                    .ThenInclude(ts => ts!.Table)
                .ToListAsync();

            return orders.Select(order => new OrderDto
            {
                OrderId = order.OrderId,
                OrderDate = order.CreatedAt,
                IsPaid = order.IsPaid,
                TableCode = order.OrderTables.FirstOrDefault(ot => ot.IsPrimary)?.Table?.TableCode ?? 
                           order.OrderTables.FirstOrDefault()?.Table?.TableCode ?? string.Empty,
                TableName = order.OrderTables.FirstOrDefault(ot => ot.IsPrimary)?.Table?.TableName ?? 
                           order.OrderTables.FirstOrDefault()?.Table?.TableName,
                AreaName = order.OrderTables.FirstOrDefault(ot => ot.IsPrimary)?.Table?.Area?.AreaName ?? 
                          order.OrderTables.FirstOrDefault()?.Table?.Area?.AreaName,
                TotalAmount = order.OrderDetails.Sum(od => od.TotalPrice),
                TableSessionId = order.TableSessionId,
                TableSession = order.TableSession != null ? new TableSessionDto
                {
                    Id = order.TableSession.Id,
                    SessionId = order.TableSession.SessionId,
                    TableId = order.TableSession.TableId,
                    OpenAt = order.TableSession.OpenAt,
                    CloseAt = order.TableSession.CloseAt,
                    OpenedBy = order.TableSession.OpenedBy,
                    ClosedBy = order.TableSession.ClosedBy,
                    Status = order.TableSession.Status
                } : null,
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
                .Include(o => o.OrderTables)
                    .ThenInclude(ot => ot.Table)
                        .ThenInclude(t => t!.Area)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Dish)
                .Include(o => o.TableSession) // Include TableSession
                    .ThenInclude(ts => ts!.Table)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            return order == null ? null : new OrderDto
            {
                OrderId = order.OrderId,
                OrderDate = order.CreatedAt,
                IsPaid = order.IsPaid,
                TableCode = order.OrderTables.FirstOrDefault(ot => ot.IsPrimary)?.Table?.TableCode ?? 
                           order.OrderTables.FirstOrDefault()?.Table?.TableCode ?? string.Empty,
                TableName = order.OrderTables.FirstOrDefault(ot => ot.IsPrimary)?.Table?.TableName ?? 
                           order.OrderTables.FirstOrDefault()?.Table?.TableName,
                AreaName = order.OrderTables.FirstOrDefault(ot => ot.IsPrimary)?.Table?.Area?.AreaName ?? 
                          order.OrderTables.FirstOrDefault()?.Table?.Area?.AreaName,
                TotalAmount = order.OrderDetails.Sum(od => od.TotalPrice),
                TableSessionId = order.TableSessionId,
                TableSession = order.TableSession != null ? new TableSessionDto
                {
                    Id = order.TableSession.Id,
                    SessionId = order.TableSession.SessionId,
                    TableId = order.TableSession.TableId,
                    OpenAt = order.TableSession.OpenAt,
                    CloseAt = order.TableSession.CloseAt,
                    OpenedBy = order.TableSession.OpenedBy,
                    ClosedBy = order.TableSession.ClosedBy,
                    Status = order.TableSession.Status
                } : null,
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
                .Include(o => o.OrderTables)
                    .ThenInclude(ot => ot.Table)
                        .ThenInclude(t => t!.Area)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Dish)
                .Include(o => o.TableSession)
                .Where(o => o.OrderTables.Any(ot => ot.Table!.TableCode == tableCode))
                .ToListAsync();

            return orders.Select(order => new OrderDto
            {
                OrderId = order.OrderId,
                OrderDate = order.CreatedAt,
                IsPaid = order.IsPaid,
                TableCode = order.OrderTables.FirstOrDefault(ot => ot.Table!.TableCode == tableCode)?.Table?.TableCode ?? string.Empty,
                TableName = order.OrderTables.FirstOrDefault(ot => ot.Table!.TableCode == tableCode)?.Table?.TableName,
                AreaName = order.OrderTables.FirstOrDefault(ot => ot.Table!.TableCode == tableCode)?.Table?.Area?.AreaName,
                TotalAmount = order.OrderDetails.Sum(od => od.TotalPrice),
                TableSessionId = order.TableSessionId
            });
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var orders = await _context.Orders
                .Include(o => o.OrderTables)
                    .ThenInclude(ot => ot.Table)
                        .ThenInclude(t => t!.Area)
                .Include(o => o.OrderDetails)
                .Include(o => o.TableSession)
                .Where(o => o.CreatedAt >= startDate && o.CreatedAt <= endDate)
                .ToListAsync();

            return orders.Select(order => new OrderDto
            {
                OrderId = order.OrderId,
                OrderDate = order.CreatedAt,
                IsPaid = order.IsPaid,
                TableCode = order.OrderTables.FirstOrDefault(ot => ot.IsPrimary)?.Table?.TableCode ?? 
                           order.OrderTables.FirstOrDefault()?.Table?.TableCode ?? string.Empty,
                TableName = order.OrderTables.FirstOrDefault(ot => ot.IsPrimary)?.Table?.TableName ?? 
                           order.OrderTables.FirstOrDefault()?.Table?.TableName,
                AreaName = order.OrderTables.FirstOrDefault(ot => ot.IsPrimary)?.Table?.Area?.AreaName ?? 
                          order.OrderTables.FirstOrDefault()?.Table?.Area?.AreaName,
                TotalAmount = order.OrderDetails.Sum(od => od.TotalPrice),
                TableSessionId = order.TableSessionId
            });
        }

        public async Task<OrderDto> CreateOrderAsync(OrderDto dto)
        {
            var entity = new Order
            {
                OrderId = dto.OrderId,
                CreatedAt = dto.OrderDate,
                IsPaid = dto.IsPaid,
                PrimaryAreaId = "A001", // Default area - should be determined from business logic
                TableSessionId = dto.TableSessionId // Set TableSessionId if provided
            };

            _context.Orders.Add(entity);
            await _context.SaveChangesAsync();

            // If TableCode is provided, create OrderTable relationship
            if (!string.IsNullOrEmpty(dto.TableCode))
            {
                var table = await _context.Tables.FirstOrDefaultAsync(t => t.TableCode == dto.TableCode);
                if (table != null)
                {
                    var orderTable = new OrderTable
                    {
                        OrderId = entity.OrderId,
                        TableId = table.TableId,
                        IsPrimary = true,
                        FromTime = DateTime.UtcNow
                    };
                    _context.OrderTables.Add(orderTable);
                    await _context.SaveChangesAsync();
                }
            }

            return dto;
        }

        public async Task<OrderDto?> UpdateOrderAsync(string id, OrderDto dto)
        {
            var entity = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == id);
            if (entity == null) return null;

            entity.CreatedAt = dto.OrderDate;
            entity.IsPaid = dto.IsPaid;
            entity.TableSessionId = dto.TableSessionId; // Update TableSessionId

            await _context.SaveChangesAsync();
            return dto;
        }

        public async Task<bool> DeleteOrderAsync(string id)
        {
            var entity = await _context.Orders
                .Include(o => o.OrderDetails)
                .Include(o => o.OrderTables)
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
                .Include(o => o.OrderTables)
                    .ThenInclude(ot => ot.Table)
                        .ThenInclude(t => t!.Area)
                .Include(o => o.OrderDetails)
                .Include(o => o.TableSession)
                .Where(o => !o.IsPaid)
                .ToListAsync();

            return orders.Select(order => new OrderDto
            {
                OrderId = order.OrderId,
                OrderDate = order.CreatedAt,
                IsPaid = order.IsPaid,
                TableCode = order.OrderTables.FirstOrDefault(ot => ot.IsPrimary)?.Table?.TableCode ?? 
                           order.OrderTables.FirstOrDefault()?.Table?.TableCode ?? string.Empty,
                TableName = order.OrderTables.FirstOrDefault(ot => ot.IsPrimary)?.Table?.TableName ?? 
                           order.OrderTables.FirstOrDefault()?.Table?.TableName,
                AreaName = order.OrderTables.FirstOrDefault(ot => ot.IsPrimary)?.Table?.Area?.AreaName ?? 
                          order.OrderTables.FirstOrDefault()?.Table?.Area?.AreaName,
                TotalAmount = order.OrderDetails.Sum(od => od.TotalPrice),
                TableSessionId = order.TableSessionId
            });
        }

        // New method to get orders by TableSessionId
        public async Task<IEnumerable<OrderDto>> GetOrdersByTableSessionIdAsync(string tableSessionId)
        {
            var orders = await _context.Orders
                .Include(o => o.OrderTables)
                    .ThenInclude(ot => ot.Table)
                        .ThenInclude(t => t!.Area)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Dish)
                .Include(o => o.TableSession)
                    .ThenInclude(ts => ts!.Table)
                .Where(o => o.TableSessionId == tableSessionId)
                .ToListAsync();

            return orders.Select(order => new OrderDto
            {
                OrderId = order.OrderId,
                OrderDate = order.CreatedAt,
                IsPaid = order.IsPaid,
                TableCode = order.OrderTables.FirstOrDefault(ot => ot.IsPrimary)?.Table?.TableCode ?? 
                           order.OrderTables.FirstOrDefault()?.Table?.TableCode ?? string.Empty,
                TableName = order.OrderTables.FirstOrDefault(ot => ot.IsPrimary)?.Table?.TableName ?? 
                           order.OrderTables.FirstOrDefault()?.Table?.TableName,
                AreaName = order.OrderTables.FirstOrDefault(ot => ot.IsPrimary)?.Table?.Area?.AreaName ?? 
                          order.OrderTables.FirstOrDefault()?.Table?.Area?.AreaName,
                TotalAmount = order.OrderDetails.Sum(od => od.TotalPrice),
                TableSessionId = order.TableSessionId,
                TableSession = order.TableSession != null ? new TableSessionDto
                {
                    Id = order.TableSession.Id,
                    SessionId = order.TableSession.SessionId,
                    TableId = order.TableSession.TableId,
                    OpenAt = order.TableSession.OpenAt,
                    CloseAt = order.TableSession.CloseAt,
                    OpenedBy = order.TableSession.OpenedBy,
                    ClosedBy = order.TableSession.ClosedBy,
                    Status = order.TableSession.Status
                } : null,
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
        public async Task<OrderDto?> GetLatestOrderDetailsByTableIdAsync(string tableId)
        {
            // Lấy session mới nhất (chưa đóng)
            var latestSession = await _context.TableSessions
                .Where(ts => ts.TableId == tableId && ts.CloseAt == null) // tương đương IS NULL
                .OrderByDescending(ts => ts.OpenAt)
                .FirstOrDefaultAsync();

            if (latestSession == null)
                return null;

            // Lấy order gắn với session đó
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Dish)
                .Include(o => o.OrderTables)
                    .ThenInclude(ot => ot.Table)
                        .ThenInclude(t => t.Area)
                .FirstOrDefaultAsync(o => o.TableSessionId == latestSession.SessionId); // Fixed: Use SessionId instead of Id

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
                TableSessionId = order.TableSessionId, // Fixed: Add missing TableSessionId
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