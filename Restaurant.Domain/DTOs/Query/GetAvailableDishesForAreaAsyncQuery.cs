using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Domain.DTOs.Query
{
    public class GetAvailableDishesForAreaAsyncQuery
    {
                public string AreaId { get; set; } = string.Empty;
        public string SearchString { get; set; } = string.Empty;
    }
}
