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

        public async Task<IEnumerable<TableSessionDto>> GetAllAsync()
        {
            var sessions = await _context.TableSessions
                .Include(ts => ts.Table)
                .ToListAsync();

            return sessions.Select(MapToDto);
        }

        public async Task<TableSessionDto?> GetByIdAsync(string id)
        {
            var session = await _context.TableSessions
                .Include(ts => ts.Table)
                .FirstOrDefaultAsync(ts => ts.Id == id);

            return session != null ? MapToDto(session) : null;
        }

        public async Task<TableSessionDto?> GetBySessionIdAsync(string sessionId)
        {
            var session = await _context.TableSessions
                .Include(ts => ts.Table)
                .FirstOrDefaultAsync(ts => ts.SessionId == sessionId);

            return session != null ? MapToDto(session) : null;
        }

        public async Task<IEnumerable<TableSessionDto>> GetByTableIdAsync(string tableId)
        {
            var sessions = await _context.TableSessions
                .Include(ts => ts.Table)
                .Where(ts => ts.TableId == tableId)
                .OrderByDescending(ts => ts.OpenAt)
                .ToListAsync();

            return sessions.Select(MapToDto);
        }

        public async Task<IEnumerable<TableSessionDto>> GetByStatusAsync(SessionStatus status)
        {
            var sessions = await _context.TableSessions
                .Include(ts => ts.Table)
                .Where(ts => ts.Status == status)
                .ToListAsync();

            return sessions.Select(MapToDto);
        }

        public async Task<TableSessionDto?> GetActiveSessionByTableIdAsync(string tableId)
        {
            var session = await _context.TableSessions
                .Include(ts => ts.Table)
                .FirstOrDefaultAsync(ts => ts.TableId == tableId && ts.Status == SessionStatus.Open);

            return session != null ? MapToDto(session) : null;
        }

        public async Task<TableSessionDto> CreateAsync(TableSessionDto tableSessionDto)
        {
            var session = new TableSession
            {
                Id = Guid.NewGuid().ToString(),
                SessionId = tableSessionDto.SessionId,
                TableId = tableSessionDto.TableId,
                OpenAt = tableSessionDto.OpenAt,
                CloseAt = tableSessionDto.CloseAt,
                OpenedBy = tableSessionDto.OpenedBy,
                ClosedBy = tableSessionDto.ClosedBy,
                Status = tableSessionDto.Status
            };

            _context.TableSessions.Add(session);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(session.Id) ?? throw new InvalidOperationException("Failed to create session");
        }

        public async Task<TableSessionDto?> UpdateAsync(string id, TableSessionDto tableSessionDto)
        {
            var session = await _context.TableSessions.FindAsync(id);
            if (session == null) return null;

            session.SessionId = tableSessionDto.SessionId;
            session.TableId = tableSessionDto.TableId;
            session.OpenAt = tableSessionDto.OpenAt;
            session.CloseAt = tableSessionDto.CloseAt;
            session.OpenedBy = tableSessionDto.OpenedBy;
            session.ClosedBy = tableSessionDto.ClosedBy;
            session.Status = tableSessionDto.Status;

            await _context.SaveChangesAsync();

            return await GetByIdAsync(id);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var session = await _context.TableSessions.FindAsync(id);
            if (session == null) return false;

            _context.TableSessions.Remove(session);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<TableSessionDto?> OpenSessionAsync(string tableId, string openedBy)
        {
            // Check if there's already an active session for this table
            var existingSession = await _context.TableSessions
                .FirstOrDefaultAsync(ts => ts.TableId == tableId && ts.Status == SessionStatus.Open);

            if (existingSession != null)
            {
                return null; // Table already has an active session
            }

            var newSession = new TableSession
            {
                Id = Guid.NewGuid().ToString(),
                SessionId = $"TS{DateTime.Now:yyyyMMddHHmmss}",
                TableId = tableId,
                OpenAt = DateTime.UtcNow,
                OpenedBy = openedBy,
                Status = SessionStatus.Open
            };

            _context.TableSessions.Add(newSession);
            
            // Update table status
            var table = await _context.Tables.FindAsync(tableId);
            if (table != null)
            {
                table.Status = TableStatus.Occupied;
                table.OpenAt = newSession.OpenAt;
            }

            await _context.SaveChangesAsync();

            return await GetByIdAsync(newSession.Id);
        }

        public async Task<TableSessionDto?> CloseSessionAsync(string sessionId, string closedBy)
        {
            var session = await _context.TableSessions
                .Include(ts => ts.Table)
                .FirstOrDefaultAsync(ts => ts.SessionId == sessionId);

            if (session == null || session.Status != SessionStatus.Open)
            {
                return null;
            }

            session.CloseAt = DateTime.UtcNow;
            session.ClosedBy = closedBy;
            session.Status = SessionStatus.Closed;

            // Update table status
            if (session.Table != null)
            {
                session.Table.Status = TableStatus.Available;
                session.Table.CloseAt = session.CloseAt;
            }

            await _context.SaveChangesAsync();

            return await GetByIdAsync(session.Id);
        }

        public async Task<IEnumerable<TableSessionDto>> GetSessionsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var sessions = await _context.TableSessions
                .Include(ts => ts.Table)
                .Where(ts => ts.OpenAt >= startDate && ts.OpenAt <= endDate)
                .OrderByDescending(ts => ts.OpenAt)
                .ToListAsync();

            return sessions.Select(MapToDto);
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
                // Id and TableId are not part of TableDto; expose client-facing fields only
                TableCode = table.TableCode,
                TableName = table.TableName,
                Capacity = table.Capacity,
                IsActive = table.IsActive,
                Status = table.Status.ToString(),
                OpenAt = table.OpenAt,
                CloseAt = table.CloseAt,
                AreaId = table.AreaId
            };
        }
    }
}