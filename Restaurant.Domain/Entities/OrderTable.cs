namespace Restaurant.Domain.Entities
{
    public class OrderTable : BaseEntity
    {
        public required string OrderId { get; set; } // FK ? Orders
        public required string TableId { get; set; } // FK ? Tables
        public bool IsPrimary { get; set; } = false; // Mark primary table
        public DateTime? FromTime { get; set; }
        public DateTime? ToTime { get; set; }

        // Navigation
        public virtual Order? Order { get; set; }
        public virtual Table? Table { get; set; }
    }
}