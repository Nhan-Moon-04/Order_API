namespace Restaurant.Domain.DTOs
{
    public class AreaDishPriceDto
    {
        public required string Id { get; set; }
        public required string AreaId { get; set; }
        public required string DishId { get; set; }
        public double CustomPrice { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string? AreaName { get; set; }
        public string? DishName { get; set; }
        public int SortOrder { get; set; } = 0;
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
