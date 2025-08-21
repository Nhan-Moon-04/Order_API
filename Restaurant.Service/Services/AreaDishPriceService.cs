using Microsoft.EntityFrameworkCore;
using Restaurant.Data;
using Restaurant.Domain.DTOs;
using Restaurant.Domain.Entities;
using Restaurant.Service.Interfaces;

namespace Restaurant.Service.Services
{
    public class AreaDishPriceService : IAreaDishPriceService
    {
        private readonly RestaurantDbContext _context;

        public AreaDishPriceService(RestaurantDbContext context)
        {
            _context = context;
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
                                     EffectiveDate = p.EffectiveDate
                                 })
                                 .ToListAsync();
        }

        public async Task<AreaDishPriceDto?> GetByIdAsync(string id)
        {
            return await _context.AreaDishPrices
                                 .Include(p => p.Area)
                                 .Include(p => p.Dish)
                                 .Where(p => p.AreaId == id)
                                 .Select(p => new AreaDishPriceDto
                                 {
                                     Id = p.Id,
                                     AreaId = p.Area != null ? p.Area.AreaId : string.Empty,
                                     DishId = p.Dish != null ? p.Dish.DishId : string.Empty,
                                     CustomPrice = p.CustomPrice,
                                     EffectiveDate = p.EffectiveDate
                                 })
                                 .FirstOrDefaultAsync();
        }

        public async Task<AreaDishPriceDto> CreateAsync(AreaDishPriceDto dto)
        {
            var entityId = Guid.NewGuid().ToString();
            var entity = new AreaDishPrices
            {
                Id = entityId,
                AreaId = dto.AreaId,
                DishId = dto.DishId,
                CustomPrice = dto.CustomPrice,
                EffectiveDate = dto.EffectiveDate,
                IsActive = true
            };

            _context.AreaDishPrices.Add(entity);
            await _context.SaveChangesAsync();

            dto.Id = entity.Id;
            return dto;
        }

        public async Task<bool> UpdateAsync(string id, AreaDishPriceDto dto)
        {
            var entity = await _context.AreaDishPrices.FirstOrDefaultAsync(e => e.Id == id);
            if (entity == null) return false;

            entity.AreaId = dto.AreaId;
            entity.DishId = dto.DishId;
            entity.CustomPrice = dto.CustomPrice;
            entity.EffectiveDate = dto.EffectiveDate;

            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var entity = await _context.AreaDishPrices.FirstOrDefaultAsync(e => e.Id == id);
            if (entity == null) return false;

            _context.AreaDishPrices.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
