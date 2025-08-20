namespace Restaurant.Domain.Entities
{
    public class OrderDetail : BaseEntity
    {
        public required string OrderDetailId { get; set; }
        public required string OrderId { get; set; }
        public required string DishId { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }   // Giá tại thời điểm order
        public double TotalPrice => Quantity * UnitPrice;

        // Navigation
        public virtual Order? Order { get; set; }
        public virtual Dishes? Dish { get; set; }
    }
}
