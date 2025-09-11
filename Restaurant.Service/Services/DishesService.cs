using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Restaurant.Data;
using Restaurant.Domain.DTOs;
using Restaurant.Domain.DTOs.Query;
using Restaurant.Domain.Entities;
using Restaurant.Service.Interfaces;
using Dapper;
using System.Data;
namespace Restaurant.Service.Services
{
    public class DishesService : IDishesService
    {
        private readonly RestaurantDbContext _context;
        public DishesService(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DishesDto>> GetAllDishesAsync()
        {
            var dishes = await _context.Dishes
                .Include(d => d.Kitchen)
                .Include(d => d.DishGroup)
                .Include(d => d.AreaDishPrices)
                .ToListAsync();

            return dishes.Select(dish => new DishesDto
            {
                DishId = dish.DishId,
                DishName = dish.DishName,
                BasePrice = dish.BasePrice,
                KitchenId = dish.KitchenId,
                GroupId = dish.GroupId,
                IsActive = dish.IsActive,
                CreatedAt = dish.CreatedAt,
                KitchenName = dish.Kitchen?.KitchenName,
                GroupName = dish.DishGroup?.GroupName
            });
        }




        public class DishesServiceDapper
        {
            private readonly string _connectionString;

            public DishesServiceDapper(IConfiguration configuration)
            {
                _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
            }

            public async Task<(IEnumerable<DishesDto> Items, int TotalRecords)> GetPagedDishesAsync(DishesQueryParameters query)
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                // query count
                var countSql = @"
        SELECT COUNT(*)
        FROM Dishes d
        LEFT JOIN DishGroups g ON d.GroupId = g.GroupId
        LEFT JOIN Kitchens k ON d.KitchenId = k.KitchenId
        WHERE (@Search = '' OR d.DishName LIKE '%' + @Search + '%')
          AND (@IsActive = -1 OR d.IsActive = @IsActive);
    ";

                var totalRecords = await connection.ExecuteScalarAsync<int>(countSql, new
                {
                    Search = query.SearchString ?? "",
                    IsActive = query.IsActive
                });

                // query data trang hiện tại
                var dataSql = @"
                    SELECT d.DishId, d.DishName, d.BasePrice, d.IsActive, 
               d.KitchenId, d.GroupId, d.CreatedAt,
               k.KitchenName, g.GroupName, d.Description
        FROM Dishes d
        LEFT JOIN DishGroups g ON d.GroupId = g.GroupId
        LEFT JOIN Kitchens k ON d.KitchenId = k.KitchenId
        WHERE (@Search = '' OR d.DishName LIKE '%' + @Search + '%')
          AND (@IsActive = -1 OR d.IsActive = @IsActive)
        ORDER BY d.CreatedAt DESC
        OFFSET (@PageIndex - 1) * @PageSize ROWS
        FETCH NEXT @PageSize ROWS ONLY;
    ";

                var items = await connection.QueryAsync<DishesDto>(dataSql, new
                {
                    Search = query.SearchString ?? "",
                    IsActive = query.IsActive,
                    PageIndex = query.PageIndex,
                    PageSize = query.PageSize
                });

                return (items, totalRecords);
            }


            public async Task<DishesDto> AddDishAsync(DishesDto newDish)
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                newDish.CreatedAt = DateTime.Now;

                var createdAt = $"TS{DateTime.Now:yyyyMMddHHmmss};";
                var insertSql = @"
                INSERT INTO Dishes (DishId, DishName, BasePrice, KitchenId, GroupId,Description, IsActive, CreatedAt)
                VALUES (@DishId, @DishName, @BasePrice, @KitchenId, @GroupId,@Description, @IsActive, @createdAt);
            ";

                await connection.ExecuteAsync(insertSql, newDish);
                return newDish;
            }



            /// <summary>
            /// 
            /// Lây danh sách món chưa có trong khu vực
            ///     
            /// </summary>
            /// 
            public async Task<IEnumerable<DishesDto>> GetAvailableDishesForAreaAsync(string areaId)
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                string sql = @"
        SELECT d.DishId, d.DishName, d.BasePrice
        FROM Dishes d
        WHERE d.DishId NOT IN (
            SELECT adp.DishId
            FROM AreaDishPrices adp
            WHERE adp.AreaId = @AreaId
        )";

                var dishes = await connection.QueryAsync<DishesDto>(sql, new { AreaId = areaId });
                return dishes;
            }

        }
    }
}