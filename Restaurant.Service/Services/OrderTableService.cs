using Microsoft.EntityFrameworkCore;
using Restaurant.Data;
using Restaurant.Domain.DTOs;
using Restaurant.Domain.Entities;
using Restaurant.Service.Interfaces;

namespace Restaurant.Service.Services
{
    public class OrderTableService : IOrderTableService
    {
        private readonly RestaurantDbContext _context;

        public OrderTableService(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OrderTableDto>> GetAllAsync()
        {
            var orderTables = await _context.OrderTables
                .Include(ot => ot.Order)
                .Include(ot => ot.Table)
                .ToListAsync();

            return orderTables.Select(MapToDto);
        }

        public async Task<OrderTableDto?> GetByIdAsync(string id)
        {
            var orderTable = await _context.OrderTables
                .Include(ot => ot.Order)
                .Include(ot => ot.Table)
                .FirstOrDefaultAsync(ot => ot.Id == id);

            return orderTable != null ? MapToDto(orderTable) : null;
        }

        public async Task<IEnumerable<OrderTableDto>> GetByOrderIdAsync(string orderId)
        {
            var orderTables = await _context.OrderTables
                .Include(ot => ot.Order)
                .Include(ot => ot.Table)
                .Where(ot => ot.OrderId == orderId)
                .ToListAsync();

            return orderTables.Select(MapToDto);
        }

        public async Task<IEnumerable<OrderTableDto>> GetByTableIdAsync(string tableId)
        {
            var orderTables = await _context.OrderTables
                .Include(ot => ot.Order)
                .Include(ot => ot.Table)
                .Where(ot => ot.TableId == tableId)
                .ToListAsync();

            return orderTables.Select(MapToDto);
        }

        public async Task<OrderTableDto> CreateAsync(OrderTableDto orderTableDto)
        {
            var orderTable = new OrderTable
            {
                Id = Guid.NewGuid().ToString(),
                OrderId = orderTableDto.OrderId,
                TableId = orderTableDto.TableId,
                IsPrimary = orderTableDto.IsPrimary,
                FromTime = orderTableDto.FromTime,
                ToTime = orderTableDto.ToTime
            };

            _context.OrderTables.Add(orderTable);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(orderTable.Id) ?? throw new InvalidOperationException("Failed to create order table");
        }

        public async Task<OrderTableDto?> UpdateAsync(string id, OrderTableDto orderTableDto)
        {
            var orderTable = await _context.OrderTables.FindAsync(id);
            if (orderTable == null) return null;

            orderTable.OrderId = orderTableDto.OrderId;
            orderTable.TableId = orderTableDto.TableId;
            orderTable.IsPrimary = orderTableDto.IsPrimary;
            orderTable.FromTime = orderTableDto.FromTime;
            orderTable.ToTime = orderTableDto.ToTime;

            await _context.SaveChangesAsync();

            return await GetByIdAsync(id);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var orderTable = await _context.OrderTables.FindAsync(id);
            if (orderTable == null) return false;

            _context.OrderTables.Remove(orderTable);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<OrderTableDto?> GetPrimaryTableByOrderIdAsync(string orderId)
        {
            var orderTable = await _context.OrderTables
                .Include(ot => ot.Order)
                .Include(ot => ot.Table)
                .FirstOrDefaultAsync(ot => ot.OrderId == orderId && ot.IsPrimary);

            return orderTable != null ? MapToDto(orderTable) : null;
        }

        public async Task<bool> SetPrimaryTableAsync(string orderId, string tableId)
        {
            // Reset all tables for this order to non-primary
            var orderTables = await _context.OrderTables
                .Where(ot => ot.OrderId == orderId)
                .ToListAsync();

            foreach (var ot in orderTables)
            {
                ot.IsPrimary = ot.TableId == tableId;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        private static OrderTableDto MapToDto(OrderTable orderTable)
        {
            return new OrderTableDto
            {
                OrderId = orderTable.OrderId,
                TableId = orderTable.TableId,
                IsPrimary = orderTable.IsPrimary,
                FromTime = orderTable.FromTime,
                ToTime = orderTable.ToTime,
                Order = orderTable.Order != null ? MapOrderToDto(orderTable.Order) : null,
                Table = orderTable.Table != null ? MapTableToDto(orderTable.Table) : null
            };
        }

        private static OrderDto MapOrderToDto(Order order)
        {
            // Map only properties that exist in OrderDto
            return new OrderDto
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
                TotalAmount = order.OrderDetails.Sum(od => od.TotalPrice)
            };
        }

        private static TableDto MapTableToDto(Table table)
        {
            return new TableDto
            {
                // Id and TableId are not part of TableDto; expose client-facing fields only
                TableCode = table.TableCode,
                TableName = table.TableName,
                Capacity = table.Capacity,
                IsActive = table.IsActive,
                Status = table.Status.ToString(),
                AreaId = table.AreaId
            };
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
                .FirstOrDefaultAsync(o => o.TableSessionId == latestSession.SessionId); // nhớ đúng field

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