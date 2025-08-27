using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Restaurant.Data;
using Restaurant.Domain.DTOs;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;
using Restaurant.Service.Interfaces;
using System.Data;
using Dapper;

namespace Restaurant.Service.Services
{
    public class TableService : ITableService
    {
        private readonly RestaurantDbContext _context;

        public TableService(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TableDto>> GetAllTablesAsync()
        {
            var tables = await _context.Tables
                .Include(t => t.Area)
                .ToListAsync();

            return tables.Select(table => new TableDto
            {
                TableCode = table.TableCode,
                TableName = table.TableName,
                Capacity = table.Capacity,
                IsActive = table.IsActive,
                AreaId = table.AreaId,
                AreaName = table.Area?.AreaName,
                Status = table.Status.ToString()
            });
        }

        public async Task<TableDto?> GetTableByIdAsync(string id)
        {
            var table = await _context.Tables
                .Include(t => t.Area)
                .FirstOrDefaultAsync(t => t.TableCode == id);

            return table == null ? null : new TableDto
            {
                TableCode = table.TableCode,
                TableName = table.TableName,
                Capacity = table.Capacity,
                IsActive = table.IsActive,
                AreaId = table.AreaId,
                AreaName = table.Area?.AreaName,
                Status = table.Status.ToString()
            };
        }

        public async Task<IEnumerable<TableDto>> GetTablesByAreaIdAsync(string areaId)
        {
            var tables = await _context.Tables
                .Include(t => t.Area)
                .Where(t => t.AreaId == areaId)
                .ToListAsync();

            return tables.Select(table => new TableDto
            {
                TableCode = table.TableCode,
                TableName = table.TableName,
                Capacity = table.Capacity,
                IsActive = table.IsActive,
                AreaId = table.AreaId,
                AreaName = table.Area?.AreaName,
                Status = table.Status.ToString()
            });
        }

        public async Task<TableDto> CreateTableAsync(TableDto dto)
        {
            var entity = new Table
            {
                TableId = Guid.NewGuid().ToString(), // Generate a unique TableId
                TableCode = dto.TableCode,
                TableName = dto.TableName,
                Capacity = dto.Capacity,
                IsActive = dto.IsActive,
                AreaId = dto.AreaId
            };

            _context.Tables.Add(entity);
            await _context.SaveChangesAsync();

            return dto;
        }

        public async Task<TableDto?> UpdateTableAsync(string id, TableDto dto)
        {
            var entity = await _context.Tables.FirstOrDefaultAsync(t => t.TableCode == id);
            if (entity == null) return null;

            entity.TableName = dto.TableName;
            entity.Capacity = dto.Capacity;
            entity.IsActive = dto.IsActive;
            entity.AreaId = dto.AreaId;

            await _context.SaveChangesAsync();
            return dto;
        }

        public async Task<bool> DeleteTableAsync(string id)
        {
            var entity = await _context.Tables.FirstOrDefaultAsync(t => t.TableCode == id);
            if (entity == null) return false;

            _context.Tables.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<TableDto>> GetAvailableTablesAsync()
        {
            var tables = await _context.Tables
                .Include(t => t.Area)
                .Where(t => t.IsActive)
                .ToListAsync();

            return tables.Select(table => new TableDto
            {
                TableCode = table.TableCode,
                TableName = table.TableName,
                Capacity = table.Capacity,
                IsActive = table.IsActive,
                AreaId = table.AreaId,
                AreaName = table.Area?.AreaName
            });
        }

        public async Task<IEnumerable<TableDto>> GetTableStatusAsync()
        {
            var tables = await _context.Tables
                .Include(t => t.Area)
                .ToListAsync();

            return tables.Select(table => new TableDto
            {
                TableCode = table.TableCode,
                TableName = table.TableName,
                Capacity = table.Capacity,
                IsActive = table.IsActive,
                AreaId = table.AreaId,
                AreaName = table.Area?.AreaName,
                Status = table.Status.ToString()  
            });
        }

        public async Task<IEnumerable<TableDto>> GetTablesByFilterAsync(string areaId, bool? isActive)
        {
            var query = _context.Tables
                .Include(t => t.Area)
                .Where(t => t.AreaId == areaId);

            if (isActive.HasValue)
            {
                query = query.Where(t => t.IsActive == isActive.Value);
            }

            var tables = await query.ToListAsync();

            return tables.Select(table => new TableDto
            {
                TableCode = table.TableCode,
                TableName = table.TableName,
                Capacity = table.Capacity,
                IsActive = table.IsActive,
                AreaId = table.AreaId,
                AreaName = table.Area?.AreaName,
                Status = table.Status.ToString()
            });
        }


        public async Task<IEnumerable<TableDto>> PostChangeStatusTable(
            string tableCode, 
            string status)
        {
            var table = await _context.Tables
                .Include(t => t.Area)
                .FirstOrDefaultAsync(t => t.TableCode == tableCode);
                
            if (table == null) return Enumerable.Empty<TableDto>();
            
            if (Enum.TryParse<TableStatus>(status, out TableStatus tableStatus))
            {
                table.Status = tableStatus;
                await _context.SaveChangesAsync();
            }
            
            return new List<TableDto>
            {
                new TableDto
                {
                    TableCode = table.TableCode,
                    TableName = table.TableName,
                    Capacity = table.Capacity,
                    IsActive = table.IsActive,
                    AreaId = table.AreaId,
                    AreaName = table.Area?.AreaName,
                    Status = table.Status.ToString()
                }
            };
        }

        public async Task<TableDto?> OpenTableAsync(string tableCode, string areaId, string? openedBy = null)
        {
            var table = await _context.Tables
                .Include(t => t.Area)
                .FirstOrDefaultAsync(t => t.TableCode == tableCode && t.AreaId == areaId);
            
            if (table == null) return null;

            // Check if table is available to open
            if (table.Status != TableStatus.Available)
            {
                throw new InvalidOperationException($"Bàn {tableCode} trong khu {areaId} không thể mở vì đang ở trạng thái {table.Status}");
            }

            // Kiểm tra xem có session đang mở không
            var existingSession = await _context.TableSessions
                .FirstOrDefaultAsync(ts => ts.TableId == table.TableId && ts.Status == SessionStatus.Available);
            
            if (existingSession != null)
            {
                throw new InvalidOperationException($"Bàn {tableCode} đã có session đang mở");
            }

            // Tạo session mới trong TableSessions
            var newSession = new TableSession
            {
                Id = Guid.NewGuid().ToString(),
                SessionId = $"TS{DateTime.Now:yyyyMMddHHmmss}_{table.TableCode}",
                TableId = table.TableId,
                OpenAt = DateTime.Now,
                OpenedBy = openedBy ?? "Unknown",
                Status = SessionStatus.Available
            };

            _context.TableSessions.Add(newSession);

            // Cập nhật status của bàn thành Occupied
            table.Status = TableStatus.Occupied;

            // Tự động tạo Order mới và liên kết với TableSession
            var newOrder = new Order
            {
                Id = Guid.NewGuid().ToString(),
                OrderId = $"ORD{DateTime.Now:yyyyMMddHHmmss}_{table.TableCode}",
                CreatedAt = DateTime.Now,
                PrimaryAreaId = table.AreaId,
                IsPaid = false,
                OrderStatus = OrderStatus.Open,
                TableSessionId = newSession.Id
            };

            _context.Orders.Add(newOrder);

            // Tạo OrderTable để liên kết Order với Table
            var orderTable = new OrderTable
            {
                Id = Guid.NewGuid().ToString(),
                OrderId = newOrder.OrderId,
                TableId = table.TableId,
                IsPrimary = true,
                FromTime = DateTime.Now
            };

            _context.OrderTables.Add(orderTable);

            await _context.SaveChangesAsync();

            return new TableDto
            {
                TableCode = table.TableCode,
                TableName = table.TableName,
                Capacity = table.Capacity,
                IsActive = table.IsActive,
                AreaId = table.AreaId,
                AreaName = table.Area?.AreaName,
                Status = table.Status.ToString()
            };
        }


        public async Task<TableDto?> CloseTableAsync(string tableCode, string? closedBy = null)
        {
            var table = await _context.Tables
                .Include(t => t.Area)
                .FirstOrDefaultAsync(t => t.TableCode == tableCode);
            
            if (table == null) return null;

            // Check if table is occupied to close
            if (table.Status != TableStatus.Occupied)
            {
                throw new InvalidOperationException($"Bàn {tableCode} không thể đóng vì không đang ở trạng thái Occupied");
            }

            // Tìm và đóng session hiện tại
            var activeSession = await _context.TableSessions
                .FirstOrDefaultAsync(ts => ts.TableId == table.TableId && ts.Status == SessionStatus.Available);
            
            if (activeSession != null)
            {
                activeSession.CloseAt = DateTime.Now;
                activeSession.ClosedBy = closedBy ?? "Unknown";
                activeSession.Status = SessionStatus.Closed;
            }

            // Update table status to Available
            table.Status = TableStatus.Available;

            await _context.SaveChangesAsync();

            return new TableDto
            {
                TableCode = table.TableCode,
                TableName = table.TableName,
                Capacity = table.Capacity,
                IsActive = table.IsActive,
                AreaId = table.AreaId,
                AreaName = table.Area?.AreaName,
                Status = table.Status.ToString()
            };
        }








        public class TableDapperService
        {
            private readonly string _connectionString;

            public TableDapperService(IConfiguration configuration)
            {
                _connectionString = configuration.GetConnectionString("DefaultConnection");
            }

            public async Task<int> GetTotalTablesAsync()
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    string sql = "SELECT COUNT(*) FROM Tables";
                    var result = await db.ExecuteScalarAsync<int>(sql);
                    return result;
                }
            }
        }

    }





}