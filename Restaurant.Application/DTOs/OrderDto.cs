using System.Text.Json.Serialization;

namespace Restaurant.Application.DTOs
{
    public class OrderDto
    {
        public string OrderId { get; set; } = string.Empty;
        
        [JsonPropertyName("orderDate")]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        
        [JsonPropertyName("isPaid")]
        public bool IsPaid { get; set; } = false;
        
        [JsonPropertyName("tableCode")]
        public string TableCode { get; set; } = string.Empty;
        
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        [JsonPropertyName("closedAt")]
        public DateTime? ClosedAt { get; set; }
        
        [JsonPropertyName("tableSessionId")]
        public string? TableSessionId { get; set; } 

        // Navigation properties for display
        [JsonPropertyName("tableName")]
        public string? TableName { get; set; }
        
        [JsonPropertyName("areaName")]
        public string? AreaName { get; set; }
        
        [JsonPropertyName("totalAmount")]
        public decimal TotalAmount { get; set; }
        
        [JsonPropertyName("orderDetails")]
        public List<OrderDetailDto>? OrderDetails { get; set; }
        
        [JsonPropertyName("tableSession")]
        public TableSessionDto? TableSession { get; set; } 
    }
}