using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Domain.DTOs.Request
{
    public class AddDishesRequest
    {
        public string DishName { get; set; } = string.Empty;
        public double BasePrice { get; set; }
        public string KitchenId { get; set; } = string.Empty;
        public string GroupId { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public string Description { get; set; } = string.Empty;
    }
}
