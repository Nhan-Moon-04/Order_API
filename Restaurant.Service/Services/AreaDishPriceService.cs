using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Restaurant.Data;
using Restaurant.Domain.DTOs;
using Restaurant.Domain.DTOs.Query;
using Restaurant.Domain.DTOs.Request;
using Restaurant.Domain.Entities;
using Restaurant.Service.Interfaces;
using System.Data;
using System.Text;

namespace Restaurant.Service.Services
{
    public class AreaDishPriceService : IAreaDishPriceService
    {
        private readonly RestaurantDbContext _context;
        private readonly string _connectionString;

        public AreaDishPriceService(RestaurantDbContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
        }

        public async Task<IEnumerable<AreaDishPriceDto>> GetAllAsync()
        {
            return await _context.AreaDishPrices
                                 .Include(p => p.Area)
                                 .Include(p => p.Dish)
                                 .Select(p => new AreaDishPriceDto
                                 {
                                     Id = p.Id,
                                     AreaId = p.Area != null ? p.Area.AreaId : string.Empty,
                                     DishId = p.Dish != null ? p.Dish.DishId : string.Empty,
                                     CustomPrice = p.CustomPrice,
                                     EffectiveDate = p.EffectiveDate,
                                     AreaName = p.Area != null ? p.Area.AreaName : null,
                                     DishName = p.Dish != null ? p.Dish.DishName : null
                                 })
                                 .ToListAsync();
        }
        public async Task<(IEnumerable<AreaDishPriceDto> Items, int TotalRecords)>
            GetPagedAreaDishPriceAsyncEF(AreaDishPriceQueryParameters query)
        {
            // Validate query parameters
            if (query.PageIndex <= 0) query.PageIndex = 1;
            if (query.PageSize <= 0) query.PageSize = 20;

            // Bắt đầu query
            var q = from adp in _context.AreaDishPrices
                    join a in _context.Areas on adp.AreaId equals a.AreaId into areaGroup
                    from a in areaGroup.DefaultIfEmpty()
                    join d in _context.Dishes on adp.DishId equals d.DishId into dishGroup
                    from d in dishGroup.DefaultIfEmpty()
                    select new AreaDishPriceDto
                    {
                        Id = adp.Id,
                        AreaId = adp.AreaId,
                        DishId = adp.DishId,
                        CustomPrice = adp.CustomPrice,
                        EffectiveDate = adp.EffectiveDate,
                        AreaName = a != null ? a.AreaName : "",
                        DishName = d != null ? d.DishName : "",
                        IsActive = adp.IsActive,
                        CreatedAt = adp.CreatedAt,
                        SortOrder = adp.SortOrder

                    };

            // --- Filter ---
            if (!string.IsNullOrEmpty(query.AreaId))
                q = q.Where(x => x.AreaId == query.AreaId);

            if (!string.IsNullOrEmpty(query.DishId))
                q = q.Where(x => x.DishId == query.DishId);

            if (query.IsActive != -1)
            {
                bool isActive = query.IsActive == 1;
                q = q.Where(x => x.IsActive == isActive);
            }

            if (query.EffectiveDateFrom.HasValue)
                q = q.Where(x => x.EffectiveDate >= query.EffectiveDateFrom.Value);

            if (query.EffectiveDateTo.HasValue)
                q = q.Where(x => x.EffectiveDate <= query.EffectiveDateTo.Value);

            // --- Search ---
            if (!string.IsNullOrEmpty(query.SearchString))
            {
                var keywords = query.SearchString.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                foreach (var kw in keywords)
                {
                    string keyword = kw; // tránh closure bug
                    q = q.Where(x =>
                        x.DishName.Contains(keyword) ||
                        x.AreaName.Contains(keyword) ||
                        x.CustomPrice.ToString().Contains(keyword));
                }
            }

            // --- Count ---
            int totalRecords = await q.CountAsync();

            // --- Paging ---
            var items = await q
                .OrderBy(x => x.SortOrder == 0 ? 1 : 0)  // NULL sortOrder xuống cuối
                .ThenBy(x => x.SortOrder)
                .Skip((query.PageIndex - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            return (items, totalRecords);
        }


        public async Task<IEnumerable<AreaDishPriceDto>> GetByIdAsync(string areaId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"
            SELECT adp.Id,
                   adp.AreaId,
                   adp.DishId,
                   adp.CustomPrice,
                   adp.EffectiveDate,
                   a.AreaName,
                   d.DishName,
                   adp.IsActive,
                   adp.CreatedAt,
                   adp.SortOrder
            FROM AreaDishPrices adp
            LEFT JOIN Areas a ON adp.AreaId = a.AreaId
            LEFT JOIN Dishes d ON adp.DishId = d.DishId
            WHERE adp.AreaId = @AreaId
            ORDER BY adp.SortOrder ASC";

                return await db.QueryAsync<AreaDishPriceDto>(sql, new { AreaId = areaId });
            }
        }




        public async Task<bool> UpdateAsync(string id, AreaDishPriceDto dto)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = @"
            UPDATE AreaDishPrices
            SET AreaId = @AreaId,
                DishId = @DishId,
                CustomPrice = @CustomPrice,
                EffectiveDate = @EffectiveDate,
                IsActive = @IsActive
            WHERE Id = @Id";

                var rows = await db.ExecuteAsync(sql, new
                {
                    Id = id,
                    AreaId = dto.AreaId,
                    DishId = dto.DishId,
                    CustomPrice = dto.CustomPrice,
                    EffectiveDate = dto.EffectiveDate,
                    IsActive = dto.IsActive
                });

                return rows > 0;
            }
        }

        public async Task<AreaDishPriceDto> UpdatePriceAsync(string id, decimal customPrice)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                // 1. Update giá
                string updateSql = @"
            UPDATE AreaDishPrices
            SET CustomPrice = @CustomPrice
            WHERE Id = @Id";

                var rows = await db.ExecuteAsync(updateSql, new
                {
                    Id = id,
                    CustomPrice = customPrice
                });

                if (rows == 0)
                    throw new InvalidOperationException($"AreaDishPrice with ID {id} not found");

                // 2. Lấy lại record sau khi update
                string selectSql = @"
            SELECT adp.Id,
                   adp.AreaId,
                   adp.DishId,
                   adp.CustomPrice,
                   adp.EffectiveDate,
                   a.AreaName,
                   d.DishName,
                   adp.IsActive
            FROM AreaDishPrices adp
            LEFT JOIN Areas a ON adp.AreaId = a.AreaId
            LEFT JOIN Dishes d ON adp.DishId = d.DishId
            WHERE adp.Id = @Id";

                var result = await db.QueryFirstOrDefaultAsync<AreaDishPriceDto>(
                    selectSql,
                    new { Id = id }
                );

                if (result == null)
                    throw new InvalidOperationException($"Failed to retrieve updated AreaDishPrice with ID {id}");

                return result;
            }
        }

        public async Task<(IEnumerable<AreaDishPriceDto> Items, int TotalRecords)>
            GetPagedAreaDishPriceAsync(AreaDishPriceQueryParameters query)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            if (query.PageIndex <= 0) query.PageIndex = 1;
            if (query.PageSize <= 0) query.PageSize = 20;

            var where = new StringBuilder("WHERE 1=1");
            var parameters = new DynamicParameters();

            parameters.Add("AreaId", query.AreaId ?? string.Empty);
            parameters.Add("DishId", query.DishId ?? string.Empty);
            parameters.Add("IsActive", query.IsActive);
            parameters.Add("EffectiveDateFrom", query.EffectiveDateFrom);
            parameters.Add("EffectiveDateTo", query.EffectiveDateTo);
            parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
            parameters.Add("PageSize", query.PageSize);

            // --- Search tổng quát ---
            if (!string.IsNullOrEmpty(query.SearchString))
            {
                var keywords = query.SearchString.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < keywords.Length; i++)
                {
                    var paramName = $"Search{i}";
                    parameters.Add(paramName, $"%{keywords[i]}%");

                    where.Append($@"
                AND (
                    d.DishName COLLATE SQL_Latin1_General_CP1253_CI_AI LIKE @{paramName}
                    OR a.AreaName COLLATE SQL_Latin1_General_CP1253_CI_AI LIKE @{paramName}
                    OR CAST(adp.CustomPrice AS NVARCHAR) LIKE @{paramName}
                )");
                }
            }

            // --- Lọc theo field riêng ---
            if (!string.IsNullOrEmpty(query.AreaId))
                where.Append(" AND adp.AreaId = @AreaId");

            if (!string.IsNullOrEmpty(query.DishId))
                where.Append(" AND adp.DishId = @DishId");

            if (query.IsActive != -1)
                where.Append(" AND adp.IsActive = @IsActive");

            if (query.EffectiveDateFrom.HasValue)
                where.Append(" AND adp.EffectiveDate >= @EffectiveDateFrom");

            if (query.EffectiveDateTo.HasValue)
                where.Append(" AND adp.EffectiveDate <= @EffectiveDateTo");

            // --- DishName search riêng ---
            if (!string.IsNullOrEmpty(query.DishName))
            {
                parameters.Add("DishName", $"%{query.DishName}%");
                where.Append(" AND d.DishName COLLATE SQL_Latin1_General_CP1253_CI_AI LIKE @DishName");
            }

            if (!string.IsNullOrEmpty(query.AreaName))
            {
                parameters.Add("AreaName", $"%{query.AreaName}%");
                where.Append(" AND a.AreaName COLLATE SQL_Latin1_General_CP1253_CI_AI LIKE @AreaName");
            }

            if (!string.IsNullOrEmpty(query.Description))
            {
                parameters.Add("Description", $"%{query.Description}%");
                where.Append(" AND d.Description COLLATE SQL_Latin1_General_CP1253_CI_AI LIKE @Description");
            }

            if (!string.IsNullOrEmpty(query.KitchenName))
            {
                parameters.Add("KitchenName", $"%{query.KitchenName}%");
                where.Append(" AND k.KitchenName COLLATE SQL_Latin1_General_CP1253_CI_AI LIKE @KitchenName");
            }

            if (!string.IsNullOrEmpty(query.GroupName))
            {
                parameters.Add("GroupName", $"%{query.GroupName}%");
                where.Append(" AND g.GroupName COLLATE SQL_Latin1_General_CP1253_CI_AI LIKE @GroupName");
            }

            // --- Count ---
            string countSql = $@"
        SELECT COUNT(*)
        FROM AreaDishPrices adp
        LEFT JOIN Areas a ON adp.AreaId = a.AreaId
        LEFT JOIN Dishes d ON adp.DishId = d.DishId
        LEFT JOIN Kitchens k ON d.KitchenId = k.KitchenId
        LEFT JOIN DishGroups g ON d.GroupId = g.GroupId
        {where}";

            int totalRecords = await connection.ExecuteScalarAsync<int>(countSql, parameters);

            // --- Data query ---
            string dataSql = $@"
        SELECT 
            adp.Id,
            adp.AreaId,
            adp.DishId,
            adp.CustomPrice,
            adp.EffectiveDate,
            ISNULL(a.AreaName, '') as AreaName,
            ISNULL(d.DishName, '') as DishName,
            ISNULL(d.Description, '') as Description,
            ISNULL(k.KitchenName, '') as KitchenName,
            ISNULL(g.GroupName, '') as GroupName,
            adp.IsActive,
            adp.CreatedAt,
            ISNULL(adp.SortOrder, 0) as SortOrder
        FROM AreaDishPrices adp
        LEFT JOIN Areas a ON adp.AreaId = a.AreaId
        LEFT JOIN Dishes d ON adp.DishId = d.DishId
        LEFT JOIN Kitchens k ON d.KitchenId = k.KitchenId
        LEFT JOIN DishGroups g ON d.GroupId = g.GroupId
        {where}
        ORDER BY 
            CASE WHEN adp.SortOrder IS NULL THEN 1 ELSE 0 END,
            adp.SortOrder ASC
        OFFSET @Offset ROWS
        FETCH NEXT @PageSize ROWS ONLY";

            var items = await connection.QueryAsync<AreaDishPriceDto>(dataSql, parameters);

            return (items, totalRecords);
        }








        public async Task AddDishesToAreaAsync(AddAreaDishPriceRequest request)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string sql = @"
        INSERT INTO AreaDishPrices (Id, AreaId, DishId, CustomPrice, EffectiveDate, IsActive, CreatedAt)
        VALUES (@Id, @AreaId, @DishId, @CustomPrice, GETDATE(), 1, GETDATE())
    ";

            foreach (var dishId in request.DishIds)
            {
                var id = Guid.NewGuid().ToString();

                // Lấy SortOrder lớn nhất hiện tại trong Area
                var sortOrder = await connection.ExecuteScalarAsync<int>(
                    "SELECT ISNULL(MAX(SortOrder), 0) + 1 FROM AreaDishPrices WHERE AreaId = @AreaId",
                    new { AreaId = request.AreaId }
                );

                await connection.ExecuteAsync(@"
        INSERT INTO AreaDishPrices 
            (Id, AreaId, DishId, CustomPrice, SortOrder, EffectiveDate, IsActive, CreatedAt)
        VALUES 
            (@Id, @AreaId, @DishId, @CustomPrice, @SortOrder, GETDATE(), 1, GETDATE())",
                    new
                    {
                        Id = id,
                        AreaId = request.AreaId,
                        DishId = dishId,
                        CustomPrice = request.CustomPrice ?? 0,
                        SortOrder = sortOrder
                    });
            }

        }

        // Pseudocode / Plan:
        // 1. Replace erroneous catch block that tried to call StatusCode (not available in service).
        // 2. Keep behavior: attempt to delete record by Id using Dapper and return true if rows affected > 0.
        // 3. On exception, swallow/log (no logger available) and return false so method signature Task<bool> is respected.
        // 4. Ensure method compiles in a plain service class (no ASP.NET controller helpers).


        public async Task<bool> DeleteAsync(string id)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    string sql = "DELETE FROM AreaDishPrices WHERE Id = @Id";
                    var rows = await db.ExecuteAsync(sql, new { Id = id });
                    return rows > 0;
                }
            }
            catch (Exception)
            {
                // An error occurred while attempting to delete; return false.
                // If you have a logging mechanism, log the exception here.
                return false;
            }
        }
        public async Task<IEnumerable<string>> GetDishNames(string id, string search)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();


            var sql = @"
            SELECT DishName 
            FROM Dishes 
            INNER JOIN AreaDishPrices
                ON AreaDishPrices.DishId = Dishes.DishId  
               AND AreaDishPrices.AreaId = @id
            WHERE Dishes.DishName COLLATE Vietnamese_CI_AI LIKE @search";

            var dishNames = await connection.QueryAsync<string>(sql, new { id, search = $"%{search}%" });


            return dishNames;
        }
    }
}