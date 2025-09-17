using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Domain.DTOs.Request
{
    public class UpdateDishRequest
    {
        public required string DishId { get; set; }
        public required string DishName { get; set; }
        public required double BasePrice { get; set; }
        public required string KitchenId { get; set; }
        public required string GroupId { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
