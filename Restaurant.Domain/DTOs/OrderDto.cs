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

        // Navigation properties for display
        public string? TableName { get; set; }
        public string? AreaName { get; set; }
        public double TotalAmount { get; set; }
        public List<OrderDetailDto>? OrderDetails { get; set; }
    }
}