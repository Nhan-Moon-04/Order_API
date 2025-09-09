using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Domain.DTOs.Request
{
    public class UpdateIsActiveRrequest
    {
        public required string Id { get; set; }
        public required bool IsActive { get; set; }
    }
}
