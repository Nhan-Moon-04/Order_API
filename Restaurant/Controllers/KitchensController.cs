using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restaurant.Data;
using Restaurant.Domain.Entities;

namespace Restaurant.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KitchensController : ControllerBase
    {
        private readonly RestaurantDbContext _context;

        public KitchensController(RestaurantDbContext context)
        {
            _context = context;
        }

       
    }
}
