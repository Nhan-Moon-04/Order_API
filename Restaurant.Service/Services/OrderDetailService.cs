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

        #region GET
        public async Task<IEnumerable<OrderDetailDto>> GetAllOrderDetailsAsync()
        {
            var orderDetails = await _context.OrderDetails
                .Include(od => od.Dish)
                    .ThenInclude(d => d!.Kitchen)
                .ToListAsync();

            return orderDetails.Select(od => ToDto(od));
        }

        public async Task<OrderDetailDto?> GetOrderDetailByIdAsync(string id)
        {
            var od = await _context.OrderDetails
                .Include(x => x.Dish)
                    .ThenInclude(d => d!.Kitchen)
                .FirstOrDefaultAsync(x => x.OrderDetailId == id);

            return od == null ? null : ToDto(od);
        }

        public async Task<IEnumerable<OrderDetailDto>> GetOrderDetailsByOrderIdAsync(string orderId)
        {
            var list = await _context.OrderDetails
                .Include(od => od.Dish)
                    .ThenInclude(d => d!.Kitchen)
                .Where(od => od.OrderId == orderId)
                .ToListAsync();

            return list.Select(od => ToDto(od));
        }
        #endregion

        #region ADD
        public async Task<OrderDetailDto> AddFood(OrderDetailDto dto)
        {
            // Kiểm tra Order
            var order = await _context.Orders
                .Include(o => o.OrderTables)
                    .ThenInclude(ot => ot.Table)
                .FirstOrDefaultAsync(o => o.OrderId == dto.OrderId);

            if (order == null)
                throw new ArgumentException($"Order với ID {dto.OrderId} không tồn tại");

            // Kiểm tra Dish
            var dish = await _context.Dishes
                .Include(d => d.Kitchen)
                .FirstOrDefaultAsync(d => d.DishId == dto.DishId && d.IsActive);

            if (dish == null)
                throw new ArgumentException($"Món ăn với ID {dto.DishId} không tồn tại hoặc đã bị vô hiệu hóa");

            // Tính giá
            double unitPrice = dish.BasePrice;
            var primaryTable = order.OrderTables.FirstOrDefault(ot => ot.IsPrimary);
            string? areaId = primaryTable?.Table?.AreaId ?? order.PrimaryAreaId;
            PriceSource priceSource = PriceSource.Base;

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

            // Nếu món đã có trong order thì update số lượng
            var existing = await _context.OrderDetails
                .FirstOrDefaultAsync(od => od.OrderId == dto.OrderId && od.DishId == dto.DishId);

            if (existing != null)
            {
                existing.Quantity += dto.Quantity;
                existing.UnitPrice = unitPrice;
                existing.PriceSource = priceSource;
                await _context.SaveChangesAsync();
                return ToDto(existing, dish);
            }

            // Nếu chưa có thì thêm mới
            var entity = new OrderDetail
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

            _context.OrderDetails.Add(entity);
            await _context.SaveChangesAsync();
            return ToDto(entity, dish);
        }

        public async Task<OrderDetailDto> AddFoodToOrder(string orderId, string dishId, int quantity = 1)
        {
            return await AddFood(new OrderDetailDto
            {
                OrderDetailId = Guid.NewGuid().ToString(),
                OrderId = orderId,
                DishId = dishId,
                Quantity = quantity
            });
        }

        public async Task<IEnumerable<OrderDetailDto>> AddMultipleFoodsToOrder(string orderId, Dictionary<string, int> dishQuantities)
        {
            var results = new List<OrderDetailDto>();
            foreach (var kvp in dishQuantities)
            {
                var result = await AddFood(new OrderDetailDto
                {
                    OrderId = orderId,
                    DishId = kvp.Key,
                    Quantity = kvp.Value
                });
                results.Add(result);
            }
            return results;
        }
        #endregion

        #region UPDATE / REMOVE
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

        public async Task<OrderDetailDto?> UpdateFoodQuantity(string orderId, string dishId, int delta)
        {
            var entity = await _context.OrderDetails
                .Include(od => od.Dish)
                .FirstOrDefaultAsync(od => od.OrderId == orderId && od.DishId == dishId);

            if (entity == null)
                throw new ArgumentException($"Món {dishId} không tồn tại trong order {orderId}");

            entity.Quantity += delta;
            if (entity.Quantity <= 0)
            {
                _context.OrderDetails.Remove(entity);
                await _context.SaveChangesAsync();
                return null;
            }

            await _context.SaveChangesAsync();
            return ToDto(entity);
        }

        public async Task<bool> DeleteOrderDetailAsync(string id)
        {
            var entity = await _context.OrderDetails.FirstOrDefaultAsync(od => od.OrderDetailId == id);
            if (entity == null) return false;

            _context.OrderDetails.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteFoodFromOrder(string orderId, string dishId)
        {
            var entity = await _context.OrderDetails
                .FirstOrDefaultAsync(od => od.OrderId == orderId && od.DishId == dishId);

            if (entity == null)
                throw new ArgumentException($"Món {dishId} không tồn tại trong order {orderId}");

            _context.OrderDetails.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        // RemoveFood methods implementation
        public async Task<OrderDetailDto?> RemoveFood(OrderDetailDto dto)
        {
            // 1. Validation: Check if Order exists
            var order = await _context.Orders
                .Include(o => o.OrderTables)
                    .ThenInclude(ot => ot.Table)
                .FirstOrDefaultAsync(o => o.OrderId == dto.OrderId);

            if (order == null)
                throw new ArgumentException($"Order với ID {dto.OrderId} không tồn tại");

            // 2. Validation: Check if Dish exists
            var dish = await _context.Dishes
                .Include(d => d.Kitchen)
                .FirstOrDefaultAsync(d => d.DishId == dto.DishId);

            if (dish == null)
                throw new ArgumentException($"Món ăn với ID {dto.DishId} không tồn tại");

            // 3. Check if this dish exists in the order
            var existingOrderDetail = await _context.OrderDetails
                .Include(od => od.Dish)
                    .ThenInclude(d => d!.Kitchen)
                .FirstOrDefaultAsync(od => od.OrderId == dto.OrderId && od.DishId == dto.DishId);

            if (existingOrderDetail == null)
                throw new ArgumentException($"Món ăn {dish.DishName} không có trong order này");

            // 4. Check if quantity to remove is valid
            if (dto.Quantity <= 0)
                throw new ArgumentException("Số lượng cần xóa phải lớn hơn 0");

            if (dto.Quantity >= existingOrderDetail.Quantity)
            {
                // Remove the entire order detail if quantity to remove >= existing quantity
                _context.OrderDetails.Remove(existingOrderDetail);
                await _context.SaveChangesAsync();
                
                // Return null to indicate the item was completely removed
                return null;
            }
            else
            {
                // Reduce the quantity
                existingOrderDetail.Quantity -= dto.Quantity;
                await _context.SaveChangesAsync();

                // Return updated DTO
                return ToDto(existingOrderDetail);
            }
        }

        public async Task<OrderDetailDto?> RemoveFoodFromOrder(string orderId, string dishId, int quantity = 1)
        {
            var dto = new OrderDetailDto
            {
                OrderId = orderId,
                DishId = dishId,
                Quantity = quantity
            };

            return await RemoveFood(dto);
        }

        public async Task<IEnumerable<OrderDetailDto?>> RemoveMultipleFoodsFromOrder(string orderId, Dictionary<string, int> dishQuantities)
        {
            var results = new List<OrderDetailDto?>();

            foreach (var kvp in dishQuantities)
            {
                var dto = new OrderDetailDto
                {
                    OrderId = orderId,
                    DishId = kvp.Key,
                    Quantity = kvp.Value
                };

                var result = await RemoveFood(dto);
                results.Add(result);
            }

            return results;
        }
        #endregion

        #region TOTAL
        public async Task<double> GetOrderTotalAsync(string orderId)
        {
            var list = await _context.OrderDetails
                .Where(od => od.OrderId == orderId)
                .ToListAsync();

            return list.Sum(od => od.TotalPrice);
        }
        #endregion

        #region HELPER
        private static OrderDetailDto ToDto(OrderDetail entity, Dishes? dish = null)
        {
            return new OrderDetailDto
            {
                OrderDetailId = entity.OrderDetailId,
                OrderId = entity.OrderId,
                DishId = entity.DishId,
                Quantity = entity.Quantity,
                UnitPrice = entity.UnitPrice,
                DishName = dish?.DishName ?? entity.Dish?.DishName,
                KitchenName = dish?.Kitchen?.KitchenName ?? entity.Dish?.Kitchen?.KitchenName
            };
        }
        #endregion
    }
}
