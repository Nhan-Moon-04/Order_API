namespace Restaurant.Domain.DTOs.Request
{
    public class RemoveFoodRequest
    {
        public string OrderId { get; set; } = string.Empty;
        public string DishId { get; set; } = string.Empty;
        public int Quantity { get; set; } = 1;
        public string? Reason { get; set; } // Optional: Lý do xóa (khách h?y, sai order, etc.)
    }
}