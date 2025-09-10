using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restaurant.Data;
using Restaurant.Domain.DTOs;
using Restaurant.Domain.Entities;
using Restaurant.Service.Interfaces;
namespace Restaurant.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KitchensController : ControllerBase
    {
        private readonly IKitchensService _kitchen;

        public KitchensController(IKitchensService context)
        {
            _kitchen = context;
        }


    }
}
