using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Domain.Entities
{
    public class Areas: BaseEntity
    {
        public required string AreaId { get; set; }
        public required string AreaName { get; set; }
        public required string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation
        public virtual ICollection<AreaDishPrices> AreaDishPrices { get; set; } = new List<AreaDishPrices>();
    }
}
