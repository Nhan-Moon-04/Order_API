using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Domain.DTOs.Request
{
    public class GetAvailableDishesForAreaAsyncRequest
    {
        public required string AreaId { get; set; }

    }
}
