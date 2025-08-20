namespace Restaurant.Domain.DTOs
{
    public class OrderDetailDto
    {
        public string OrderDetailId { get; set; } = string.Empty;
        public string OrderId { get; set; } = string.Empty;
        public string DishId { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double TotalPrice => Quantity * UnitPrice;
        
        // Navigation properties for display
        public string? DishName { get; set; }
        public string? KitchenName { get; set; }
    }
}