using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Domain.DTOs.Request
{
    public class MoveRequest
    {
        public required TableDto Table { get; set; }
        public required string  Direction { get; set; }
    }
}
