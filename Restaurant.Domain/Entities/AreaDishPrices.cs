using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Domain.Entities
{
    public class AreaDishPrices: BaseEntity
    {
        public required string AreaId { get; set; }
        public required string DishId { get; set; }
        public double CustomPrice { get; set; }
        public DateTime EffectiveDate { get; set; }
        public bool IsActive { get; set; }

        // Navigation properties
        public virtual Areas? Area { get; set; }
        public virtual Dishes? Dish { get; set; }
    }
}
