using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Domain.DTOs.Request
{
    public class UpdateOrderDetailPayload
    {
        public string OrderId { get; set; } = string.Empty;
        public string DishId { get; set; } = string.Empty;
        public int? Delta { get; set; } 
        public bool Delete { get; set; } = false;
    }

}
