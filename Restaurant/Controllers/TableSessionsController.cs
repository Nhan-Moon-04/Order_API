using Microsoft.AspNetCore.Mvc;
using Restaurant.Domain.DTOs;
using Restaurant.Domain.Enums;
using Restaurant.Service.Interfaces;

namespace Restaurant.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TableSessionsController : ControllerBase
    {
        private readonly ITableSessionService _tableSessionService;

        public TableSessionsController(ITableSessionService tableSessionService)
        {
            _tableSessionService = tableSessionService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TableSessionDto>>> GetAll()
        {
            var sessions = await _tableSessionService.GetAllAsync();
            return Ok(sessions);
        }

        [HttpGet("by-id/{id}")]
        public async Task<ActionResult<TableSessionDto>> GetById(string id)
        {
            var session = await _tableSessionService.GetByIdAsync(id);
            if (session == null)
                return NotFound();
            return Ok(session);
        }

        [HttpGet("session/{sessionId}")]
        public async Task<ActionResult<TableSessionDto>> GetBySessionId(string sessionId)
        {
            var session = await _tableSessionService.GetBySessionIdAsync(sessionId);
            if (session == null)
            {
                return NotFound();
            }
            return Ok(session);
        }

        [HttpGet("table/{tableId}")]
        public async Task<ActionResult<IEnumerable<TableSessionDto>>> GetByTableId(string tableId)
        {
            var sessions = await _tableSessionService.GetByTableIdAsync(tableId);
            return Ok(sessions);
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<TableSessionDto>>> GetByStatus(SessionStatus status)
        {
            var sessions = await _tableSessionService.GetByStatusAsync(status);
            return Ok(sessions);
        }

        //[HttpGet("table/{tableId}/active")]
        //public async Task<ActionResult<TableSessionDto>> GetActiveSession(string tableId)
        //{
        //    var session = await _tableSessionService.GetActiveSessionByTableIdAsync(tableId);
        //    if (session == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(session);
        //}

        [HttpGet("daterange")]
        public async Task<ActionResult<IEnumerable<TableSessionDto>>> GetByDateRange(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            var sessions = await _tableSessionService.GetSessionsByDateRangeAsync(startDate, endDate);
            return Ok(sessions);
        }

        [HttpPost]
        public async Task<ActionResult<TableSessionDto>> Create([FromBody] TableSessionDto tableSessionDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdSession = await _tableSessionService.CreateAsync(tableSessionDto);
            return CreatedAtAction(nameof(GetById), new { id = createdSession.Id }, createdSession);
        }

        [HttpPost("table/{tableId}/open")]
        public async Task<ActionResult<TableSessionDto>> OpenSession(string tableId, [FromBody] string openedBy)
        {
            var session = await _tableSessionService.OpenSessionAsync(tableId, openedBy);
            if (session == null)
            {
                return BadRequest("Table already has an active session or table not found");
            }

            return CreatedAtAction(nameof(GetById), new { id = session.Id }, session);
        }

        [HttpPut("session/{sessionId}/close")]
        public async Task<ActionResult<TableSessionDto>> CloseSession(string sessionId, [FromBody] string closedBy)
        {
            var session = await _tableSessionService.CloseSessionAsync(sessionId, closedBy);
            if (session == null)
            {
                return BadRequest("Session not found or already closed");
            }

            return Ok(session);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TableSessionDto>> Update(string id, [FromBody] TableSessionDto tableSessionDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedSession = await _tableSessionService.UpdateAsync(id, tableSessionDto);
            if (updatedSession == null)
            {
                return NotFound();
            }

            return Ok(updatedSession);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var deleted = await _tableSessionService.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpGet("{tableId}")]
        public async Task<IActionResult> GetActiveSessionByTableId(string tableId)
        {
            var session = await _tableSessionService.GetActiveSessionByTableIdAsync(tableId);
            if (session == null)
                return NotFound(new { Message = "Không tìm thấy session đang mở cho bàn này" });

            return Ok(session);
        }
    }
}