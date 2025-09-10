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

        [HttpGet("{id}")]
        public async Task<ActionResult<TableSessionDto>> GetById(string id)
        {
            var session = await _tableSessionService.GetByIdAsync(id);
            if (session == null)
            {
                return NotFound();
            }

            return Ok(session);
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


   
    }
}