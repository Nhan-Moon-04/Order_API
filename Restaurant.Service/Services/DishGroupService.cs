using Microsoft.EntityFrameworkCore;
using Restaurant.Data;
using Restaurant.Domain.DTOs;
using Restaurant.Domain.Entities;
using Restaurant.Service.Interfaces;

namespace Restaurant.Service.Services
{
    public class DishGroupService : IDishesGroupService
    {
        private readonly RestaurantDbContext _context;
        public DishGroupService(RestaurantDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<DishGroupDto>> GetAllGroup()
        {
            return await _context.DishGroups 
                .Select(dg => new DishGroupDto
                {
                    GroupId = dg.GroupId,
                    GroupName = dg.GroupName,
                    Description = dg.Description,
                    IsActive = dg.IsActive,
                    CreatedAt = dg.CreatedAt
                })
                .ToListAsync();
        }
    }
}