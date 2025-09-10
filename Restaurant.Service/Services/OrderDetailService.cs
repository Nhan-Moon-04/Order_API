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



        /// <summary>
        /// Xóa hoàn toàn món ăn khỏi order (không quan tâm số lượng)
        /// </summary>
        public async Task<bool> RemoveFood(OrderDetailDto dto)
        {
            // 1. Validation: Check if Order exists
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.OrderId == dto.OrderId);

            if (order == null)
                throw new ArgumentException($"Order với ID {dto.OrderId} không tồn tại");

            // 2. Validation: Check if Dish exists
            var dish = await _context.Dishes
                .FirstOrDefaultAsync(d => d.DishId == dto.DishId);

            if (dish == null)
                throw new ArgumentException($"Món ăn với ID {dto.DishId} không tồn tại");

            // 3. Check if this dish exists in the order
            var existingOrderDetail = await _context.OrderDetails
                .FirstOrDefaultAsync(od => od.OrderId == dto.OrderId && od.DishId == dto.DishId);

            if (existingOrderDetail == null)
                throw new ArgumentException($"Món ăn {dish.DishName} không có trong order này");

            // 4. Remove the entire order detail (complete removal)
            _context.OrderDetails.Remove(existingOrderDetail);
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Xóa hoàn toàn món ăn khỏi order (không quan tâm số lượng)
        /// </summary>
        public async Task<bool> RemoveFoodFromOrder(string orderId, string dishId)
        {
            var dto = new OrderDetailDto
            {
                OrderId = orderId,
                DishId = dishId,
            };
            return await RemoveFood(dto);
        }


        public async Task<OrderDetailDto?> ChangeQuantityFood(string orderId, string dishId, int newQuantity)
        {
            // 1. Validation
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
                throw new ArgumentException($"Order với ID {orderId} không tồn tại");

            // 2. Validations
            var dish = await _context.Dishes
                .Include(d => d.Kitchen)
                .FirstOrDefaultAsync(d => d.DishId == dishId);

            if (dish == null)
                throw new ArgumentException($"Món ăn với ID {dishId} không tồn tại");

            var existingOrderDetail = await _context.OrderDetails
                .Include(od => od.Dish)
                    .ThenInclude(d => d!.Kitchen)
                .FirstOrDefaultAsync(od => od.OrderId == orderId && od.DishId == dishId);

            if (existingOrderDetail == null)
                throw new ArgumentException($"Món ăn {dish.DishName} không có trong order này");

            if (newQuantity < 0)
                throw new ArgumentException("Số lượng không thể âm");

            if (newQuantity == 0)
            {
                _context.OrderDetails.Remove(existingOrderDetail);
                await _context.SaveChangesAsync();
                return null;
            }

            // 6. Update
            existingOrderDetail.Quantity = newQuantity;
            await _context.SaveChangesAsync();

            return ToDto(existingOrderDetail);
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
