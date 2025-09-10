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



    }
}
