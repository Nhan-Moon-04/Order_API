using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Domain.DTOs
{
    public class DishesDto
    {
        public required string DishId { get; set; }
        public required string DishName { get; set; }
        public double Price { get; set; }
        public required string KitchenId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
