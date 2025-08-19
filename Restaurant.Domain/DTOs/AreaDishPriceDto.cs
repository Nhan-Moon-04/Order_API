namespace Restaurant.Domain.DTOs
{
    public class AreaDishPriceDto
    {
        public required string Id { get; set; }
        public required string AreaId { get; set; }
        public required string DishId { get; set; }
        public double CustomPrice { get; set; }
        public DateTime EffectiveDate { get; set; }
    }
}
