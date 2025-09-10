using Microsoft.EntityFrameworkCore;
using Restaurant.Data;
using Restaurant.Domain.DTOs;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;
using Restaurant.Service.Interfaces;

namespace Restaurant.Service.Services
{
    public class TableSessionService : ITableSessionService
    {
        private readonly RestaurantDbContext _context;

        public TableSessionService(RestaurantDbContext context)
        {
            _context = context;
        }


        public async Task<TableSessionDto?> GetByIdAsync(string id)
        {
            var session = await _context.TableSessions
                .Include(ts => ts.Table)
                .FirstOrDefaultAsync(ts => ts.Id == id);

            return session != null ? MapToDto(session) : null;
        }




    

            public async Task<TableSessionDto?> OpenSessionAsync(string tableId, string openedBy)
            {

            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

            var hanoiNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);


            // 1. Kiểm tra bàn có tồn tại không
            var table = await _context.Tables
                    .Include(t => t.Area) // lấy luôn Area để dùng cho Order
                    .FirstOrDefaultAsync(t => t.TableId == tableId);

                if (table == null)
                    return null;

                // 2. Kiểm tra xem bàn đã có session đang hoạt động chưa
                var activeSession = await _context.TableSessions
                    .Where(ts => ts.TableId == tableId
                              && (ts.Status == SessionStatus.Occupied
                               || ts.Status == SessionStatus.Reserved
                               || ts.Status == SessionStatus.Cleaning))
                    .OrderByDescending(ts => ts.OpenAt)
                    .FirstOrDefaultAsync();

                if (activeSession != null)
                    return null; // Bàn đang bận

                // 3. Tạo session mới
                var newSession = new TableSession
                {
                    Id = Guid.NewGuid().ToString(),
                    SessionId = $"TS{DateTime.Now:yyyyMMddHHmmss}",
                    TableId = tableId,
                    OpenAt = hanoiNow,
                    OpenedBy = openedBy,
                    Status = SessionStatus.Occupied
                };
                _context.TableSessions.Add(newSession);

                // 4. Tạo Order gắn session này
                var newOrder = new Order
                {
                    OrderId = Guid.NewGuid().ToString(),
                    CreatedAt = hanoiNow,
                    IsPaid = false,
                    OrderStatus = OrderStatus.Open,
                    TableSessionId = newSession.SessionId,
                    PrimaryAreaId = table.AreaId  
                };
                _context.Orders.Add(newOrder);

                // 5. Gắn Order với Table
                var orderTable = new OrderTable
                {
                    OrderId = newOrder.OrderId,
                    TableId = table.TableId,
                    IsPrimary = true,
                    FromTime = hanoiNow
                };
                _context.OrderTables.Add(orderTable);

                // 6. Cập nhật trạng thái bàn
                table.Status = TableStatus.Occupied;

                // 7. Lưu thay đổi
                await _context.SaveChangesAsync();

                // 8. Trả về DTO
                return new TableSessionDto
                {
                    Id = newSession.Id,
                    SessionId = newSession.SessionId,
                    TableId = newSession.TableId,
                    OpenAt = newSession.OpenAt,
                    OpenedBy = newSession.OpenedBy,
                    Status = SessionStatus.Occupied

                };
            }


        public async Task<TableSessionDto?> CloseSessionAsync(string sessionId, string closedBy)
        {

            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

            var hanoiNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);

            var session = await _context.TableSessions
                .Include(ts => ts.Table)
                .FirstOrDefaultAsync(ts => ts.SessionId == sessionId);

            if (session == null || session.Status == SessionStatus.Closed)
            {
                return null; // không tồn tại hoặc đã đóng rồi
            }

            // 1. Cập nhật thông tin session
            session.CloseAt = hanoiNow;
            session.ClosedBy = closedBy;
            session.Status = SessionStatus.Closed;

            // 2. Đóng tất cả orders thuộc session 
            var orders = await _context.Orders
                .Where(o => o.TableSessionId == session.SessionId && o.OrderStatus == OrderStatus.Open)
                .ToListAsync();

            foreach (var order in orders)
            {
                order.OrderStatus = OrderStatus.Closed;
                order.ClosedAt = session.CloseAt;
            }

            // 3. Cập nhật OrderTable (đặt thời điểm kết thúc FromTime)
            var orderTables = await _context.OrderTables
                .Where(ot => orders.Select(o => o.OrderId).Contains(ot.OrderId) && ot.ToTime == null)
                .ToListAsync();

            foreach (var ot in orderTables)
            {
                ot.ToTime = hanoiNow;
            }

            // 4. Cập nhật trạng thái bàn
            if (session.Table != null)
            {
                session.Table.Status = TableStatus.Available;
            }

            // 5. Lưu thay đổi
            await _context.SaveChangesAsync();

            // 6. Trả về DTO
            return await GetByIdAsync(session.Id);
        }




        private static TableSessionDto MapToDto(TableSession session)
        {
            return new TableSessionDto
            {
                Id = session.Id,
                SessionId = session.SessionId,
                TableId = session.TableId,
                OpenAt = session.OpenAt,
                CloseAt = session.CloseAt,
                OpenedBy = session.OpenedBy,
                ClosedBy = session.ClosedBy,
                Status = session.Status,
                Table = session.Table != null ? MapTableToDto(session.Table) : null
            };
        }

        private static TableDto MapTableToDto(Table table)
        {
            return new TableDto
            {
               
                TableCode = table.TableCode,
                TableName = table.TableName,
                Capacity = table.Capacity,
                IsActive = table.IsActive,
                Status = table.Status.ToString(),

                AreaId = table.AreaId
            };
        }


       



    }
}