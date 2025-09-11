using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Domain.DTOs.Query
{
    public class AreaDishPriceQueryParameters
    {
        public string SearchString { get; set; } = string.Empty;
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int IsActive { get; set; } = -1;
        public string AreaId { get; set; } = string.Empty;
        public string DishId { get; set; } = string.Empty;
        public DateTime? EffectiveDateFrom { get; set; }
        public DateTime? EffectiveDateTo { get; set; }
        public string AreaName { get; set; } = string.Empty;
        public string DishName { get; set; } = string.Empty;
        public string KitchenName { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}