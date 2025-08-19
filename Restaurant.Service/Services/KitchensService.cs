using Microsoft.EntityFrameworkCore;
using Restaurant.Data;
using Restaurant.Domain.DTOs;
using Restaurant.Domain.Entities;
using Restaurant.Service.Interfaces;

namespace Restaurant.Service.Services
{
    public class KitchensService : IKitchensService
    {

        public readonly RestaurantDbContext _context;

        public KitchensService(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<KitchensDto>> GetAllAsync()
        {
            return await _context.Kitchens
                .Select(k => new KitchensDto
                {
                    KitchenId = k.KitchenId,
                    KitchenName = k.KitchenName,
                    Description = k.Description,
                    IsActive = k.IsActive,
                    CreatedAt = k.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<KitchensDto> CreateAsync(KitchensDto dto)
        {
            var entityId = Guid.NewGuid().ToString();
            var entity = new Kitchens
            {
                KitchenId = entityId,
                Id = entityId,
                KitchenName = dto.KitchenName,
                Description = dto.Description,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _context.Kitchens.Add(entity);
            await _context.SaveChangesAsync();

            dto.KitchenId = entity.KitchenId;
            return dto;
        }


        public async Task<KitchensDto> CreateDishAsync(KitchensDto dto)
        {
            var entityId = Guid.NewGuid().ToString();
            var entity = new Kitchens
            {
                KitchenId = entityId,
                Id = entityId,
                KitchenName = dto.KitchenName,
                Description = dto.Description,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _context.Kitchens.Add(entity);
            await _context.SaveChangesAsync();

            dto.KitchenId = entity.KitchenId;
            return dto;
        }
        public async Task<bool> UpdateAsync(string id, KitchensDto dto)
        {
            var kitchen = await _context.Kitchens.FindAsync(id);
            if (kitchen == null) return false;
            kitchen.KitchenName = dto.KitchenName;
            kitchen.Description = dto.Description;
            kitchen.IsActive = dto.IsActive;
            _context.Kitchens.Update(kitchen);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var kitchen = await _context.Kitchens.FindAsync(id);
            if (kitchen == null) return false;
            _context.Kitchens.Remove(kitchen);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<KitchensDto?> GetByIdAsync(string kitchenId)
        {
            var kitchen = await _context.Kitchens
                .FirstOrDefaultAsync(k => k.KitchenId == kitchenId);

            if (kitchen == null) return null;

            return new KitchensDto
            {
                KitchenId = kitchen.KitchenId,
                KitchenName = kitchen.KitchenName,
                Description = kitchen.Description,
                IsActive = kitchen.IsActive,
                CreatedAt = kitchen.CreatedAt
            };
        }

    }
}
