using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Domain.Entities
{
    public class Dishes: BaseEntity
    {
        public required string DishId { get; set; }
        public required string DishName { get; set; }
        public double Price { get; set; }
        public required string KitchenId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation
        public virtual Kitchens? Kitchen { get; set; }
        public virtual ICollection<AreaDishPrices> AreaDishPrices { get; set; } = new List<AreaDishPrices>();
    }
}
