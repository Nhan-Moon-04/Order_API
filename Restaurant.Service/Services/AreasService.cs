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
    public class AreasService : IAreasService
    {
        private readonly RestaurantDbContext _context;
        public AreasService(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AreasDto>> GetAllAsync()
        {
            return await _context.Areas.Select(a => new AreasDto
            {
                AreaId = a.AreaId,
                AreaName = a.AreaName,
                Description = a.Description,
                IsActive = a.IsActive,
                CreatedAt = a.CreatedAt
            }).ToListAsync();
        }

        public async Task<AreasDto?> GetByIdAsync(string id)
        {
            return await _context.Areas
                .Where(a => a.AreaId == id)
                .Select(a => new AreasDto
                {
                    AreaId = a.AreaId,
                    AreaName = a.AreaName,
                    Description = a.Description,
                    IsActive = a.IsActive,
                    CreatedAt = a.CreatedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<AreasDto> CreateAsync(AreasDto dto)
        {
            var areaId = Guid.NewGuid().ToString();
            var area = new Areas
            {
                Id = areaId,
                AreaId = areaId,
                AreaName = dto.AreaName,
                Description = dto.Description,
                IsActive = dto.IsActive,
                CreatedAt = dto.CreatedAt
            };

            _context.Areas.Add(area);
            await _context.SaveChangesAsync();
            dto.AreaId = area.AreaId;
            return dto;
        }


        public async Task<int> CountAresa()
        {
            var count = await _context.Areas.CountAsync();
            return count;
        }




        public class AreasDapperService
        {
            private readonly string _connectionString;

            public AreasDapperService(IConfiguration configuration)
            {
                _connectionString = configuration.GetConnectionString("DefaultConnection");
            }
             
            public async Task<AreasDto> UpdateIsActivate(string id, bool active)
            {
                using var connection = new Microsoft.Data.SqlClient.SqlConnection(_connectionString);
                var sql = "UPDATE Areas SET IsActive = @Active WHERE AreaId = @Id; " +
                          "SELECT AreaId, AreaName, Description, IsActive, CreatedAt FROM Areas WHERE AreaId = @Id;";
                var area = await connection.QuerySingleOrDefaultAsync<AreasDto>(sql, new { Id = id, Active = active });
                if (area == null)
                {
                    throw new KeyNotFoundException($"Không tìm thấy khu vực với ID: {id}");
                }
                return area;
            }
        }
    }
}
