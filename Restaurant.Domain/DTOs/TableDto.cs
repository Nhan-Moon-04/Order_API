namespace Restaurant.Domain.DTOs
{
    public class TableDto
    {
        public string TableCode { get; set; } = string.Empty;

        public string TableName { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public bool IsActive { get; set; } = true;
        public string AreaId { get; set; } = string.Empty;

        public string Status { get; set; } = "Available"; // Default status

        // Navigation properties for display
        public string? AreaName { get; set; }
    }
}