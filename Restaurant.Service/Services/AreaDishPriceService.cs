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

            // Validate query parameters
            if (query.PageIndex <= 0) query.PageIndex = 1;
            if (query.PageSize <= 0) query.PageSize = 20;

            // --- WHERE động ---
            var where = new StringBuilder("WHERE 1=1");

            if (!string.IsNullOrEmpty(query.SearchString))
            {
                where.Append(@"
        AND (
            d.DishName LIKE '%' + @Search + '%'
            OR a.AreaName LIKE '%' + @Search + '%'
            OR CAST(adp.CustomPrice AS NVARCHAR) LIKE '%' + @Search + '%'
        )");
            }

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

            // Create parameters object
            var parameters = new
            {
                Search = query.SearchString ?? string.Empty,
                AreaId = query.AreaId ?? string.Empty,
                DishId = query.DishId ?? string.Empty,
                IsActive = query.IsActive,
                EffectiveDateFrom = query.EffectiveDateFrom,
                EffectiveDateTo = query.EffectiveDateTo,
                Offset = (query.PageIndex - 1) * query.PageSize,
                PageSize = query.PageSize
            };

            // --- Đếm tổng ---
            string countSql = $@"
        SELECT COUNT(*)
        FROM AreaDishPrices adp
        LEFT JOIN Areas a ON adp.AreaId = a.AreaId
        LEFT JOIN Dishes d ON adp.DishId = d.DishId
        {where}";

            int totalRecords = await connection.ExecuteScalarAsync<int>(countSql, parameters);

            // --- Lấy dữ liệu phân trang ---
            string dataSql = $@"
        SELECT 
            adp.Id,
            adp.AreaId,
            adp.DishId,
            adp.CustomPrice,
            adp.EffectiveDate,
            ISNULL(a.AreaName, '') as AreaName,
            ISNULL(d.DishName, '') as DishName,
            adp.IsActive,
            adp.CreatedAt,
            ISNULL(adp.SortOrder, 0) as SortOrder
        FROM AreaDishPrices adp
        LEFT JOIN Areas a ON adp.AreaId = a.AreaId
        LEFT JOIN Dishes d ON adp.DishId = d.DishId
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

        public async Task<bool> DeleteAsync(string id)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sql = "DELETE FROM AreaDishPrices WHERE Id = @Id";
                var rows = await db.ExecuteAsync(sql, new { Id = id });
                return rows > 0;
            }





        }
    }
}