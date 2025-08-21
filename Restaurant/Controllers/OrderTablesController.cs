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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderTableDto>>> GetAll()
        {
            var orderTables = await _orderTableService.GetAllAsync();
            return Ok(orderTables);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderTableDto>> GetById(string id)
        {
            var orderTable = await _orderTableService.GetByIdAsync(id);
            if (orderTable == null)
            {
                return NotFound();
            }
            return Ok(orderTable);
        }

        [HttpGet("order/{orderId}")]
        public async Task<ActionResult<IEnumerable<OrderTableDto>>> GetByOrderId(string orderId)
        {
            var orderTables = await _orderTableService.GetByOrderIdAsync(orderId);
            return Ok(orderTables);
        }

        [HttpGet("table/{tableId}")]
        public async Task<ActionResult<IEnumerable<OrderTableDto>>> GetByTableId(string tableId)
        {
            var orderTables = await _orderTableService.GetByTableIdAsync(tableId);
            return Ok(orderTables);
        }

        [HttpGet("order/{orderId}/primary")]
        public async Task<ActionResult<OrderTableDto>> GetPrimaryTable(string orderId)
        {
            var orderTable = await _orderTableService.GetPrimaryTableByOrderIdAsync(orderId);
            if (orderTable == null)
            {
                return NotFound();
            }
            return Ok(orderTable);
        }

        [HttpPost]
        public async Task<ActionResult<OrderTableDto>> Create([FromBody] OrderTableDto orderTableDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdOrderTable = await _orderTableService.CreateAsync(orderTableDto);
            return CreatedAtAction(nameof(GetById), new { id = createdOrderTable.TableId }, createdOrderTable);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<OrderTableDto>> Update(string id, [FromBody] OrderTableDto orderTableDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedOrderTable = await _orderTableService.UpdateAsync(id, orderTableDto);
            if (updatedOrderTable == null)
            {
                return NotFound();
            }

            return Ok(updatedOrderTable);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var deleted = await _orderTableService.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPut("order/{orderId}/primary/{tableId}")]
        public async Task<ActionResult> SetPrimaryTable(string orderId, string tableId)
        {
            var success = await _orderTableService.SetPrimaryTableAsync(orderId, tableId);
            if (!success)
            {
                return BadRequest("Failed to set primary table");
            }

            return NoContent();
        }
    }
}