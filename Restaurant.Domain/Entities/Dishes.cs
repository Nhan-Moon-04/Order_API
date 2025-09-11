namespace Restaurant.Domain.Entities
{
    public class Dishes : BaseEntity
    {
        public required string DishId { get; set; } // PK
        public required string DishName { get; set; }
        public double BasePrice { get; set; }
        public bool IsActive { get; set; } = true;
        public string ? Description { get; set; }
        public required string KitchenId { get; set; } // FK
        public required string GroupId { get; set; } // FK to DishGroup
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public virtual Kitchens? Kitchen { get; set; }
        public virtual DishGroup? DishGroup { get; set; }
        public virtual ICollection<AreaDishPrices> AreaDishPrices { get; set; } = new List<AreaDishPrices>();
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
