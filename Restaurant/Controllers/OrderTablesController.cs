using Microsoft.AspNetCore.Mvc;
using Restaurant.Domain.DTOs;
using Restaurant.Service.Interfaces;

namespace Restaurant.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderTablesController : ControllerBase
    {
        private readonly IOrderTableService _orderTableService;

        public OrderTablesController(IOrderTableService orderTableService)
        {
            _orderTableService = orderTableService;
        }

     
       
    }
}