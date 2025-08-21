namespace Restaurant.Domain.Entities
{
    // Giá riêng theo từng khu vực
    public class AreaDishPrices : BaseEntity
    {
        public required string AreaId { get; set; } // FK → Areas
        public required string DishId { get; set; } // FK → Dishes
        public double CustomPrice { get; set; }
        public DateTime EffectiveDate { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        // Navigation
        public virtual Areas? Area { get; set; }
        public virtual Dishes? Dish { get; set; }
    }
}
