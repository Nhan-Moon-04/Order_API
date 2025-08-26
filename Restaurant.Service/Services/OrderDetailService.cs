using Microsoft.EntityFrameworkCore;
using Restaurant.Data;
using Restaurant.Domain.DTOs;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;
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

        public async Task<OrderDetailDto> AddFood(OrderDetailDto dto)
        {
            // 1. Validation: Check if Order exists
            var order = await _context.Orders
                .Include(o => o.OrderTables)
                    .ThenInclude(ot => ot.Table)
                .FirstOrDefaultAsync(o => o.OrderId == dto.OrderId);

            if (order == null)
                throw new ArgumentException($"Order với ID {dto.OrderId} không tồn tại");

            // 2. Validation: Check if Dish exists and is active
            var dish = await _context.Dishes
                .Include(d => d.Kitchen)
                .FirstOrDefaultAsync(d => d.DishId == dto.DishId && d.IsActive);

            if (dish == null)
                throw new ArgumentException($"Món ăn với ID {dto.DishId} không tồn tại hoặc đã bị vô hiệu hóa");

            // 3. Determine pricing based on area
            double unitPrice = dish.BasePrice; // Default to base price
            var primaryTable = order.OrderTables.FirstOrDefault(ot => ot.IsPrimary);
            string? areaId = primaryTable?.Table?.AreaId ?? order.PrimaryAreaId;
            PriceSource priceSource = PriceSource.Base;

            // Check for area-specific pricing
            if (!string.IsNullOrEmpty(areaId))
            {
                var areaDishPrice = await _context.AreaDishPrices
                    .FirstOrDefaultAsync(adp => adp.AreaId == areaId 
                                              && adp.DishId == dto.DishId 
                                              && adp.IsActive 
                                              && adp.EffectiveDate <= DateTime.UtcNow);

                if (areaDishPrice != null)
                {
                    unitPrice = areaDishPrice.CustomPrice;
                    priceSource = PriceSource.Custom;
                }
            }

            // 4. Check if this dish already exists in the order
            var existingOrderDetail = await _context.OrderDetails
                .FirstOrDefaultAsync(od => od.OrderId == dto.OrderId && od.DishId == dto.DishId);

            if (existingOrderDetail != null)
            {
                // Update quantity instead of creating new record
                existingOrderDetail.Quantity += dto.Quantity;
                existingOrderDetail.UnitPrice = unitPrice; // Update to current price
                existingOrderDetail.PriceSource = priceSource;

                await _context.SaveChangesAsync();

                // Return updated DTO
                return new OrderDetailDto
                {
                    OrderDetailId = existingOrderDetail.OrderDetailId,
                    OrderId = existingOrderDetail.OrderId,
                    DishId = existingOrderDetail.DishId,
                    Quantity = existingOrderDetail.Quantity,
                    UnitPrice = existingOrderDetail.UnitPrice,
                    DishName = dish.DishName,
                    KitchenName = dish.Kitchen?.KitchenName
                };
            }

            // 5. Create new OrderDetail
            var newOrderDetail = new OrderDetail
            {
                Id = Guid.NewGuid().ToString(),
                OrderDetailId = dto.OrderDetailId ?? Guid.NewGuid().ToString(),
                OrderId = dto.OrderId,
                DishId = dto.DishId,
                Quantity = dto.Quantity,
                UnitPrice = unitPrice,
                TableId = primaryTable?.TableId,
                AreaId = areaId,
                PriceSource = priceSource
            };

            _context.OrderDetails.Add(newOrderDetail);
            await _context.SaveChangesAsync();

            // 6. Return complete DTO with navigation properties
            var result = await _context.OrderDetails
                .Include(od => od.Dish)
                    .ThenInclude(d => d!.Kitchen)
                .FirstOrDefaultAsync(od => od.OrderDetailId == newOrderDetail.OrderDetailId);

            return new OrderDetailDto
            {
                OrderDetailId = result!.OrderDetailId,
                OrderId = result.OrderId,
                DishId = result.DishId,
                Quantity = result.Quantity,
                UnitPrice = result.UnitPrice,
                DishName = result.Dish?.DishName,
                KitchenName = result.Dish?.Kitchen?.KitchenName
            };
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

        // Overloaded AddFood method for easier usage
        public async Task<OrderDetailDto> AddFoodToOrder(string orderId, string dishId, int quantity = 1)
        {
            var dto = new OrderDetailDto
            {
                OrderDetailId = Guid.NewGuid().ToString(),
                OrderId = orderId,
                DishId = dishId,
                Quantity = quantity
            };

            return await AddFood(dto);
        }

        // Method to add multiple dishes at once
        public async Task<IEnumerable<OrderDetailDto>> AddMultipleFoodsToOrder(string orderId, Dictionary<string, int> dishQuantities)
        {
            var results = new List<OrderDetailDto>();

            foreach (var kvp in dishQuantities)
            {
                var dto = new OrderDetailDto
                {
                    OrderId = orderId,
                    DishId = kvp.Key,
                    Quantity = kvp.Value
                };

                var result = await AddFood(dto);
                results.Add(result);
            }

            return results;
        }
    }
}