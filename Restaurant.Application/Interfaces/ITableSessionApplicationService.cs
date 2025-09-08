using Restaurant.Application.DTOs;

namespace Restaurant.Application.Interfaces
{
    public interface ITableSessionApplicationService
    {
        Task<IEnumerable<TableSessionDto>> GetAllAsync();
        Task<TableSessionDto?> GetByIdAsync(string id);
        Task<TableSessionDto> CreateAsync(TableSessionDto dto);
        Task<TableSessionDto?> UpdateAsync(string id, TableSessionDto dto);
        Task<bool> DeleteAsync(string id);
        Task<TableSessionDto?> OpenSessionAsync(string tableId, string openedBy);
        Task<bool> CloseSessionAsync(string sessionId, string closedBy);
    }
}