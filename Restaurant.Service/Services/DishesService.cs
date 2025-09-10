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




    }
}