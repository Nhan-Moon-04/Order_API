namespace Restaurant.Domain.DTOs
{
    public class OrderDto
    {
        public string OrderId { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public bool IsPaid { get; set; } = false;
        public string TableCode { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ClosedAt { get; set; }
        public string? TableSessionId { get; set; } // New property for TableSession reference

        // Navigation properties for display
        public string? TableName { get; set; }
        public string? AreaName { get; set; }
        public double TotalAmount { get; set; }
        public List<OrderDetailDto>? OrderDetails { get; set; }
        public TableSessionDto? TableSession { get; set; } // TableSession information
    }
}