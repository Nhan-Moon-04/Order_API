using Microsoft.AspNetCore.Mvc;
using Restaurant.Domain.DTOs;
using Restaurant.Domain.DTOs.Request;
using Restaurant.Service.Interfaces;
using static Restaurant.Service.Services.TableService;

namespace Restaurant.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TablesController : ControllerBase
    {
private readonly ITableService _tableService;
private readonly TableDapperService _service;

public TablesController(ITableService tableService, TableDapperService service)
{
    _tableService = tableService;
    _service = service;
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





        /// <summary>
        /// Mở bàn - Chuyển trạng thái từ Available sang Occupied
        /// </summary>
        /// <param name="tableCode">Mã bàn cần mở</param>
        /// <param name="areaId">Mã khu vực của bàn</param>
        /// <param name="openedBy">Tên người mở bàn (tùy chọn)</param>
        /// <returns>Thông tin bàn sau khi mở</returns>
        [HttpPost("{tableCode}/open")]
        public async Task<ActionResult<TableDto>> OpenTable(string tableCode, [FromQuery] string areaId, [FromQuery] string? openedBy = null)
        {
            try
            {
                var result = await _tableService.OpenTableAsync(tableCode, areaId, openedBy);
                if (result == null)
                {
                    return NotFound($"Không tìm thấy bàn với mã: {tableCode} trong khu vực: {areaId}");
                }
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi mở bàn: {ex.Message}");
            }
        }

        /// <summary>
        /// Đóng bàn - Chuyển trạng thái từ Occupied sang Available
        /// </summary>
        /// <param name="tableCode">Mã bàn cần đóng</param>
        /// <param name="closedBy">Tên người đóng bàn (tùy chọn)</param>
        /// <returns>Thông tin bàn sau khi đóng</returns>
        [HttpPost("{tableCode}/close")]
        public async Task<ActionResult<TableDto>> CloseTable(string tableCode, [FromQuery] string? closedBy = null)
        {
            try
            {
                var result = await _tableService.CloseTableAsync(tableCode, closedBy);
                if (result == null)
                {
                    return NotFound($"Không tìm thấy bàn với mã: {tableCode}");
                }
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi đóng bàn: {ex.Message}");
            }
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> CountTables()
        {
            var tables = await _tableService.GetAllTablesAsync();
            var count = tables.Count();
            return Ok(count);
        }











        /// <summary>
        /// /////////////////////////////
        /// Dapper Section
        /// </summary>

        [HttpPost("move/{id}/{direction}")]
        public async Task<IActionResult> Move(string id, string direction)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(direction))
                return BadRequest("Thiếu tham số id hoặc direction.");

            var tables = await _service.Move(id, direction);
            return Ok(tables);
        }



        [HttpPost("ViewTable")]
        public async Task<ActionResult<IEnumerable<TableDto>>> FilterTables([FromBody] TableFilterRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.AreaId))
                return BadRequest("AreaId is required.");

            var tables = await _service.GetTablesByFilterAsync(request.AreaId, request.IsActive);
            return Ok(tables);
        }
    }
}