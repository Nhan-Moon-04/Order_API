namespace Restaurant.Domain.Entities
{
    public class Order : BaseEntity
    {
        public required string OrderId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public bool IsPaid { get; set; } = false;

        // FK - references TableCode (T001, T002, etc.), not the GUID Id
        public required string TableCode { get; set; }

        // Navigation
        public virtual Table? Table { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
