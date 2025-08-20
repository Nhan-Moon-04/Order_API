using Microsoft.EntityFrameworkCore;
using Restaurant.Data;
using Restaurant.Domain.DTOs;
using Restaurant.Domain.Entities;
using Restaurant.Service.Interfaces;

namespace Restaurant.Service.Services
{
    public class OrderDetailService : IOrderDetailService
    {
        private readonly RestaurantDbContext _context;

        public OrderDetailService(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OrderDetailDto>> GetAllOrderDetailsAsync()
        {
            var orderDetails = await _context.OrderDetails
                .Include(od => od.Dish)
                    .ThenInclude(d => d!.Kitchen)
                .ToListAsync();

            return orderDetails.Select(od => new OrderDetailDto
            {
                OrderDetailId = od.OrderDetailId,
                OrderId = od.OrderId,
                DishId = od.DishId,
                Quantity = od.Quantity,
                UnitPrice = od.UnitPrice,
                DishName = od.Dish?.DishName,
                KitchenName = od.Dish?.Kitchen?.KitchenName
            });
        }

        public async Task<OrderDetailDto?> GetOrderDetailByIdAsync(string id)
        {
            var orderDetail = await _context.OrderDetails
                .Include(od => od.Dish)
                    .ThenInclude(d => d!.Kitchen)
                .FirstOrDefaultAsync(od => od.OrderDetailId == id);

            return orderDetail == null ? null : new OrderDetailDto
            {
                OrderDetailId = orderDetail.OrderDetailId,
                OrderId = orderDetail.OrderId,
                DishId = orderDetail.DishId,
                Quantity = orderDetail.Quantity,
                UnitPrice = orderDetail.UnitPrice,
                DishName = orderDetail.Dish?.DishName,
                KitchenName = orderDetail.Dish?.Kitchen?.KitchenName
            };
        }

        public async Task<IEnumerable<OrderDetailDto>> GetOrderDetailsByOrderIdAsync(string orderId)
        {
            var orderDetails = await _context.OrderDetails
                .Include(od => od.Dish)
                    .ThenInclude(d => d!.Kitchen)
                .Where(od => od.OrderId == orderId)
                .ToListAsync();

            return orderDetails.Select(od => new OrderDetailDto
            {
                OrderDetailId = od.OrderDetailId,
                OrderId = od.OrderId,
                DishId = od.DishId,
                Quantity = od.Quantity,
                UnitPrice = od.UnitPrice,
                DishName = od.Dish?.DishName,
                KitchenName = od.Dish?.Kitchen?.KitchenName
            });
        }

        public async Task<OrderDetailDto> CreateOrderDetailAsync(OrderDetailDto dto)
        {
            var entityId = Guid.NewGuid().ToString();
            var entity = new OrderDetail
            {
                Id = entityId,
                OrderDetailId = dto.OrderDetailId,
                OrderId = dto.OrderId,
                DishId = dto.DishId,
                Quantity = dto.Quantity,
                UnitPrice = dto.UnitPrice
            };

            _context.OrderDetails.Add(entity);
            await _context.SaveChangesAsync();

            return dto;
        }

        public async Task<OrderDetailDto?> UpdateOrderDetailAsync(string id, OrderDetailDto dto)
        {
            var entity = await _context.OrderDetails.FirstOrDefaultAsync(od => od.OrderDetailId == id);
            if (entity == null) return null;

            entity.DishId = dto.DishId;
            entity.Quantity = dto.Quantity;
            entity.UnitPrice = dto.UnitPrice;

            await _context.SaveChangesAsync();
            return dto;
        }

        public async Task<bool> DeleteOrderDetailAsync(string id)
        {
            var entity = await _context.OrderDetails.FirstOrDefaultAsync(od => od.OrderDetailId == id);
            if (entity == null) return false;

            _context.OrderDetails.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<double> GetOrderTotalAsync(string orderId)
        {
            var orderDetails = await _context.OrderDetails
                .Where(od => od.OrderId == orderId)
                .ToListAsync();

            return orderDetails.Sum(od => od.TotalPrice);
        }
    }
}