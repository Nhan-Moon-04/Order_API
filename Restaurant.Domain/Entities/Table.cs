using Restaurant.Domain.Enums;

namespace Restaurant.Domain.Entities
{
    public class Table : BaseEntity
    {
        public required string TableId { get; set; } // Using TableId as PK instead of GUID
        public required string TableCode { get; set; } // "T005" - unique
        public required string TableName { get; set; }
        public int Capacity { get; set; }
        public bool IsActive { get; set; } = true; // Added IsActive property
        public TableStatus Status { get; set; } = TableStatus.Available;
        public DateTime? OpenAt { get; set; } // NULL when not open
        public DateTime? CloseAt { get; set; } // NULL when not closed

        // FK - references AreaId (A001, A002, etc.)
        public required string AreaId { get; set; }

        // Navigation
        public virtual Areas? Area { get; set; }
        public virtual ICollection<TableSession> TableSessions { get; set; } = new List<TableSession>();
        public virtual ICollection<OrderTable> OrderTables { get; set; } = new List<OrderTable>();
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
