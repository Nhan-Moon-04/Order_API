namespace Restaurant.Domain.Entities
{
    // Giá riêng theo từng khu vực
    public class AreaDishPrices : BaseEntity
    {
        public required string AreaId { get; set; } // FK → Areas
        public required string DishId { get; set; } // FK → Dishes
        public double CustomPrice { get; set; }
        public DateTime EffectiveDate { get; set; } 
        public bool IsActive { get; set; } = true;

        public int SortOrder { get; set; } = 0;
        public DateTime CreatedAt { get; set; }


        // Navigation
        public virtual Areas? Area { get; set; }
        public virtual Dishes? Dish { get; set; }
    }
}
