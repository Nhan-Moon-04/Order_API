using Restaurant.Domain.Enums;

namespace Restaurant.Domain.Entities
{
    public class Order : BaseEntity
    {
        public required string OrderId { get; set; } // PK
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ClosedAt { get; set; }
        public bool IsPaid { get; set; } = false;
        public required string PrimaryAreaId { get; set; } // FK → Areas - for primary area determination
        public OrderStatus OrderStatus { get; set; } = OrderStatus.Open;

        // Navigation
        public virtual Areas? PrimaryArea { get; set; }
        public virtual ICollection<OrderTable> OrderTables { get; set; } = new List<OrderTable>();
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
