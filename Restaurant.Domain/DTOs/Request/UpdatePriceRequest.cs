using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Domain.DTOs.Request
{
    public class UpdatePriceRequest
    {
        public string Id { get; set; } = string.Empty;
        public decimal CustomPrice { get; set; }
    }
}
