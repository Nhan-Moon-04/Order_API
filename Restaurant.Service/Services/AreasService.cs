using Microsoft.EntityFrameworkCore;
using Restaurant.Data;
using Restaurant.Domain.DTOs;
using Restaurant.Domain.Entities;
using Restaurant.Service.Interfaces;
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
    }
}
