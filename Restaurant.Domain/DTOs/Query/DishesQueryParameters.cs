using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Domain.DTOs.Query
{
    public class DishesQueryParameters
    {
        public string SearchString { get; set; } = string.Empty;
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int IsActive { get; set; } = -1; 
    }
}
