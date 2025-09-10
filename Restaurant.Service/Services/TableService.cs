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
                .OrderBy(t => t.SortOrder)
                .ToListAsync();

            return tables.Select(table => new TableDto
            {
                TableCode = table.TableCode,
                TableName = table.TableName,
                Capacity = table.Capacity,
                IsActive = table.IsActive,
                AreaId = table.AreaId,
                AreaName = table.Area?.AreaName,
                SortOrder = table.SortOrder,
                Status = table.Status.ToString()
            });
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
                SortOrder = table.SortOrder,
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
                SortOrder = table.SortOrder,
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

            public async Task<IEnumerable<TableDto>> Move(string id, string direction)
            {
                using (var db = new SqlConnection(_connectionString))
                {
                    direction = direction.ToLower();

                    // Lấy SortOrder + AreaId của bàn hiện tại
                    var current = await db.QuerySingleAsync<(int SortOrder, string AreaId)>(
                        "SELECT SortOrder, AreaId FROM Tables WHERE TableCode = @Id",
                        new { Id = id });

                    var currentSort = current.SortOrder;
                    var areaId = current.AreaId;

                    // Lấy min/max trong cùng Area
                    var minSort = await db.ExecuteScalarAsync<int>(
                        "SELECT MIN(SortOrder) FROM Tables WHERE AreaId = @AreaId",
                        new { AreaId = areaId });

                    var maxSort = await db.ExecuteScalarAsync<int>(
                        "SELECT MAX(SortOrder) FROM Tables WHERE AreaId = @AreaId",
                        new { AreaId = areaId });

                    string otherTableCode = null;

                    if (direction == "up" && currentSort > minSort)
                    {
                        otherTableCode = await db.ExecuteScalarAsync<string>(
                            "SELECT TableCode FROM Tables WHERE AreaId = @AreaId AND SortOrder = @SortOrder",
                            new { AreaId = areaId, SortOrder = currentSort - 1 });
                    }
                    else if (direction == "down" && currentSort < maxSort)
                    {
                        otherTableCode = await db.ExecuteScalarAsync<string>(
                            "SELECT TableCode FROM Tables WHERE AreaId = @AreaId AND SortOrder = @SortOrder",
                            new { AreaId = areaId, SortOrder = currentSort + 1 });
                    }

                    if (string.IsNullOrEmpty(otherTableCode))
                    {
                        return await db.QueryAsync<TableDto>(
                            "SELECT * FROM Tables WHERE AreaId = @AreaId ORDER BY SortOrder",
                            new { AreaId = areaId });
                    }

                    await db.OpenAsync();
                    using (var tran = db.BeginTransaction())
                    {
                        if (direction == "up")
                        {
                            await db.ExecuteAsync(
                                "UPDATE Tables SET SortOrder = @SortOrder WHERE TableCode = @Id",
                                new { SortOrder = currentSort - 1, Id = id }, tran);

                            await db.ExecuteAsync(
                                "UPDATE Tables SET SortOrder = @SortOrder WHERE TableCode = @Id",
                                new { SortOrder = currentSort, Id = otherTableCode }, tran);
                        }
                        else if (direction == "down")
                        {
                            await db.ExecuteAsync(
                                "UPDATE Tables SET SortOrder = @SortOrder WHERE TableCode = @Id",
                                new { SortOrder = currentSort + 1, Id = id }, tran);

                            await db.ExecuteAsync(
                                "UPDATE Tables SET SortOrder = @SortOrder WHERE TableCode = @Id",
                                new { SortOrder = currentSort, Id = otherTableCode }, tran);
                        }

                        tran.Commit();
                    }

                    return await db.QueryAsync<TableDto>(
                        "SELECT * FROM Tables WHERE AreaId = @AreaId ORDER BY SortOrder",
                        new { AreaId = areaId });
                }
            }


            public async Task<IEnumerable<TableDto>> GetTablesByFilterAsync(string areaId, bool? isActive)
            {
                               using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    string sql = "SELECT * FROM Tables WHERE AreaId = @AreaId ";
                    if (isActive.HasValue)
                    {
                        sql += " AND IsActive = @IsActive";
                    }
                    sql += " ORDER BY SortOrder";  
                    var tables = await db.QueryAsync<TableDto>(sql, new { AreaId = areaId, IsActive = isActive });
                    return tables;
                }
            }

        }

    }





}