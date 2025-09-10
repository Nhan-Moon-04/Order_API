using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Restaurant.Data;
using Restaurant.Domain.DTOs;
using Restaurant.Domain.Entities;
using Restaurant.Service.Interfaces;
using System.Data;

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
                       adp.IsActive
                FROM AreaDishPrices adp
                LEFT JOIN Areas a ON adp.AreaId = a.AreaId
                LEFT JOIN Dishes d ON adp.DishId = d.DishId
                WHERE adp.AreaId = @AreaId";

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




    }
}
