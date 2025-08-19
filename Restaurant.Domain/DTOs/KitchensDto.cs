using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Domain.DTOs
{
    public class KitchensDto
    {
        public required string KitchenId { get; set; }
        public required string KitchenName { get; set; }    
        public required string Description { get; set; }
        public required bool IsActive { get; set; }
        public required DateTime CreatedAt { get; set; }
    }
}
