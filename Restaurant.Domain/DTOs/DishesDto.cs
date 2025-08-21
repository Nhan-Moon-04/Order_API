using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Domain.DTOs
{
    public class DishesDto
    {
        public required string DishId { get; set; }
        public required string DishName { get; set; }
        public double BasePrice { get; set; }
        public required string KitchenId { get; set; }
        public required string GroupId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // Navigation properties for display
        public string? KitchenName { get; set; }
        public string? GroupName { get; set; }
    }
}
