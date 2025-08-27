namespace Restaurant.Domain.DTOs.Request
{
    public class ChangeQuantityRequest
    {
        public string OrderId { get; set; } = string.Empty;
        public string DishId { get; set; } = string.Empty;
        public int NewQuantity { get; set; }
    }
}