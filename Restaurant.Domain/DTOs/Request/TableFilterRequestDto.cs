using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Domain.DTOs.Request
{
    public class TableFilterRequestDto
    {
        public string AreaId { get; set; } = string.Empty;
        public bool? IsActive { get; set; }  
    }
}
