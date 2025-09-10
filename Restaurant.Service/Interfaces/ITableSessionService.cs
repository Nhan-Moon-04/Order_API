using Restaurant.Domain.DTOs;
using Restaurant.Domain.Enums;

namespace Restaurant.Service.Interfaces
{
    public interface ITableSessionService
    {
        Task<TableSessionDto?> GetByIdAsync(string id);

        Task<TableSessionDto?> OpenSessionAsync(string tableId, string openedBy);
        Task<TableSessionDto?> CloseSessionAsync(string sessionId, string closedBy);

    }
}