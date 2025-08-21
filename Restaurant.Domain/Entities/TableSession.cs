using Restaurant.Domain.Enums;

namespace Restaurant.Domain.Entities
{
    public class TableSession : BaseEntity
    {
        public required string SessionId { get; set; } // PK
        public required string TableId { get; set; } // FK ? Tables
        public DateTime OpenAt { get; set; }
        public DateTime? CloseAt { get; set; }
        public string? OpenedBy { get; set; } // Staff member
        public string? ClosedBy { get; set; } // Staff member
        public SessionStatus Status { get; set; } = SessionStatus.Open;

        // Navigation
        public virtual Table? Table { get; set; }
    }
}