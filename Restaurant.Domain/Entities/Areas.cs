namespace Restaurant.Domain.Entities
{
    public class Areas : BaseEntity
    {
        public required string AreaId { get; set; }
        public required string AreaName { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public virtual ICollection<Table> Tables { get; set; } = new List<Table>();
        public virtual ICollection<AreaDishPrices> AreaDishPrices { get; set; } = new List<AreaDishPrices>();
        public virtual ICollection<Order> PrimaryOrders { get; set; } = new List<Order>();
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
