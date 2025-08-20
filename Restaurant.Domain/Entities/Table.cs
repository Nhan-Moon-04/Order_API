namespace Restaurant.Domain.Entities
{
    public class Table : BaseEntity
    {
        public required string TableCode { get; set; } // "T005"
        public required string TableName { get; set; }
        public int Capacity { get; set; }
        public bool IsActive { get; set; } = true;

        // FK - references AreaId (A001, A002, etc.), not the GUID Id
        public required string AreaId { get; set; }

        // Navigation
        public virtual Areas? Area { get; set; }
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
