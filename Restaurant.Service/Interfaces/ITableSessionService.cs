using Restaurant.Domain.DTOs;
using Restaurant.Domain.Enums;

namespace Restaurant.Service.Interfaces
{
    public interface ITableSessionService
    {
        Task<IEnumerable<TableSessionDto>> GetAllAsync();
        Task<TableSessionDto?> GetByIdAsync(string id);
        Task<TableSessionDto?> GetBySessionIdAsync(string sessionId);
        Task<IEnumerable<TableSessionDto>> GetByTableIdAsync(string tableId);
        Task<IEnumerable<TableSessionDto>> GetByStatusAsync(SessionStatus status);
        Task<TableSessionDto?> GetActiveSessionByTableIdAsync(string tableId);
        Task<TableSessionDto> CreateAsync(TableSessionDto tableSessionDto);
        Task<TableSessionDto?> UpdateAsync(string id, TableSessionDto tableSessionDto);
        Task<bool> DeleteAsync(string id);
        Task<TableSessionDto?> OpenSessionAsync(string tableId, string openedBy);
        Task<TableSessionDto?> CloseSessionAsync(string sessionId, string closedBy);
        Task<IEnumerable<TableSessionDto>> GetSessionsByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}