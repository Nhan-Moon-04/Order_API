namespace Restaurant.Domain.Entities
{
    public class Dishes : BaseEntity
    {
        public required string DishId { get; set; }
        public required string DishName { get; set; }
        public double BasePrice { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // FK
        public required string KitchenId { get; set; }

        // Navigation
        public virtual Kitchens? Kitchen { get; set; }
        public virtual ICollection<AreaDishPrices> AreaDishPrices { get; set; } = new List<AreaDishPrices>();
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
