using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Domain.DTOs.Request
{
    public class AddFoodRequest
    {
        public required string OrderId { get; set; }
        public required string DishId { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
