using Microsoft.EntityFrameworkCore;
using Restaurant.Data;
using Restaurant.Domain.DTOs;
using Restaurant.Domain.Entities;
using Restaurant.Service.Interfaces;
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

        public async Task<DishesDto> GetDishByIdAsync(string id)
        {
            var dish = await _context.Dishes
                .Include(d => d.Kitchen)
                .Include(d => d.DishGroup)
                .Include(d => d.AreaDishPrices)
                .FirstOrDefaultAsync(d => d.DishId == id);

            return dish == null ? null : new DishesDto
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
            };
        }

        public async Task<DishesDto> CreateDishAsync(DishesDto dto)
        {
            var entityId = Guid.NewGuid().ToString();
            var entity = new Dishes
            {
                DishId = entityId,
                DishName = dto.DishName,
                BasePrice = dto.BasePrice,
                KitchenId = dto.KitchenId,
                GroupId = dto.GroupId,
                IsActive = dto.IsActive,
                CreatedAt = dto.CreatedAt,
                Id = entityId
            };

            _context.Dishes.Add(entity);
            await _context.SaveChangesAsync();

            dto.DishId = entity.DishId;
            return dto;
        }
    }
}