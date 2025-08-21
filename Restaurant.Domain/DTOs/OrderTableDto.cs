namespace Restaurant.Domain.DTOs
{
    public class OrderTableDto
    {
        public string OrderId { get; set; } = string.Empty;
        public string TableId { get; set; } = string.Empty;
        public bool IsPrimary { get; set; } = false;
        public DateTime? FromTime { get; set; }
        public DateTime? ToTime { get; set; }

        // Navigation properties
        public OrderDto? Order { get; set; }
        public TableDto? Table { get; set; }
    }
}