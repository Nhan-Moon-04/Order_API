using Restaurant.Domain.Enums;

namespace Restaurant.Domain.DTOs
{
    public class TableSessionDto
    {
        public string Id { get; set; } = string.Empty;
        public string SessionId { get; set; } = string.Empty;
        public string TableId { get; set; } = string.Empty;
        public DateTime OpenAt { get; set; }
        public DateTime? CloseAt { get; set; }
        public string? OpenedBy { get; set; }
        public string? ClosedBy { get; set; }
        public SessionStatus Status { get; set; } = SessionStatus.Open;

        // Navigation properties
        public TableDto? Table { get; set; }
    }
}