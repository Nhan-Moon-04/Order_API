using Restaurant.Domain.Entities;

namespace Restaurant.Application.Interfaces.Repositories
{
    public interface ITableSessionRepository
    {
        Task<IEnumerable<TableSession>> GetAllAsync();
        Task<TableSession?> GetByIdAsync(string id);
        Task<TableSession> CreateAsync(TableSession session);
        Task<TableSession?> UpdateAsync(string id, TableSession session);
        Task<bool> DeleteAsync(string id);
        Task<TableSession?> GetActiveSessionByTableIdAsync(string tableId);
    }
}