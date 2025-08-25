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

        // New FK to TableSession
        public string? TableSessionId { get; set; } // FK → TableSession (nullable vì có thể order không liên kết với session cụ thể)

        // Navigation
        public virtual Areas? PrimaryArea { get; set; }
        public virtual TableSession? TableSession { get; set; } // Navigation property to TableSession
        public virtual ICollection<OrderTable> OrderTables { get; set; } = new List<OrderTable>();
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
