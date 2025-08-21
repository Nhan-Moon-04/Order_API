using Restaurant.Domain.Enums;

namespace Restaurant.Domain.Entities
{
    public class OrderDetail : BaseEntity
    {
        public required string OrderDetailId { get; set; } // PK
        public required string OrderId { get; set; } // FK → Orders
        public required string DishId { get; set; } // FK → Dishes
        public int Quantity { get; set; }
        public double UnitPrice { get; set; } // Frozen price at order time
        public string? TableId { get; set; } // FK → Tables, nullable if using AreaId directly
        public string? AreaId { get; set; } // FK → Areas, nullable
        public PriceSource PriceSource { get; set; } = PriceSource.Base;
        public double TotalPrice => Quantity * UnitPrice;

        // Navigation
        public virtual Order? Order { get; set; }
        public virtual Dishes? Dish { get; set; }
        public virtual Table? Table { get; set; }
        public virtual Areas? Area { get; set; }
    }
}
