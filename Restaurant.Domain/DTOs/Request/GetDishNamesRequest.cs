using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Domain.DTOs.Request
{
    public class GetDishNamesRequest
    {
        public string SearchString { get; set; } = string.Empty;
        public string SearchName { get; set; } = string.Empty;
    }
}
