using Microsoft.AspNetCore.Mvc;
using Restaurant.Domain.DTOs;
using Restaurant.Service.Interfaces;

namespace Restaurant.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TablesController : ControllerBase
    {
        private readonly ITableService _tableService;

        public TablesController(ITableService tableService)
        {
            _tableService = tableService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TableDto>>> GetAllTables()
        {
            var tables = await _tableService.GetAllTablesAsync();
            return Ok(tables);
        }

        [HttpGet("{tableCode}")]
        public async Task<ActionResult<TableDto>> GetTable(string tableCode)
        {
            var table = await _tableService.GetTableByIdAsync(tableCode);
            if (table == null)
            {
                return NotFound();
            }
            return Ok(table);
        }

        [HttpGet("area/{areaId}")]
        public async Task<ActionResult<IEnumerable<TableDto>>> GetTablesByArea(string areaId)
        {
            var tables = await _tableService.GetTablesByAreaIdAsync(areaId);
            return Ok(tables);
        }

        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<TableDto>>> GetAvailableTables()
        {
            var tables = await _tableService.GetAvailableTablesAsync();
            return Ok(tables);
        }

        [HttpPost]
        public async Task<ActionResult<TableDto>> CreateTable(TableDto tableDto)
        {
            var createdTable = await _tableService.CreateTableAsync(tableDto);
            return CreatedAtAction(nameof(GetTable), new { tableCode = createdTable.TableCode }, createdTable);
        }

        [HttpPut("{tableCode}")]
        public async Task<IActionResult> UpdateTable(string tableCode, TableDto tableDto)
        {
            var updatedTable = await _tableService.UpdateTableAsync(tableCode, tableDto);
            if (updatedTable == null)
            {
                return NotFound();
            }
            return Ok(updatedTable);
        }

        [HttpDelete("{tableCode}")]
        public async Task<IActionResult> DeleteTable(string tableCode)
        {
            var result = await _tableService.DeleteTableAsync(tableCode);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}