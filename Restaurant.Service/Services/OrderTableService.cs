using Microsoft.EntityFrameworkCore;
using Restaurant.Data;
using Restaurant.Domain.DTOs;
using Restaurant.Domain.Entities;
using Restaurant.Service.Interfaces;

namespace Restaurant.Service.Services
{
    public class OrderTableService : IOrderTableService
    {
        private readonly RestaurantDbContext _context;

        public OrderTableService(RestaurantDbContext context)
        {
            _context = context;
        }


  

      




    } 
}