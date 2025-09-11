using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Domain.DTOs.Request
{
    public class AddAreaDishPriceRequest
    {
        public string AreaId { get; set; }
        public List<string> DishIds { get; set; } = new();
        public decimal? CustomPrice { get; set; } 
    }

}
