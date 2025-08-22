using Restaurant.Domain.DTOs;

namespace Restaurant.Service.Interfaces
{
    public interface ITableService
    {
        Task<IEnumerable<TableDto>> GetAllTablesAsync();
        Task<TableDto?> GetTableByIdAsync(string tableCode);
        Task<IEnumerable<TableDto>> GetTablesByAreaIdAsync(string areaId);
        Task<TableDto> CreateTableAsync(TableDto dto);
        Task<TableDto?> UpdateTableAsync(string tableCode, TableDto dto);
        Task<bool> DeleteTableAsync(string tableCode);
        Task<IEnumerable<TableDto>> GetAvailableTablesAsync();
        Task<IEnumerable<TableDto>> GetTablesByFilterAsync(string areaId, bool? isActive);
        Task<IEnumerable<TableDto>> GetTableStatusAsync();

        Task<IEnumerable<TableDto>> PostChangeStatusTable(
            string tableCode, 
            string status, 
            DateTime? openAt, 
            DateTime? closeAt);

        Task<TableDto?> OpenTableAsync(string tableCode, string areaId, string? openedBy = null);
        Task<TableDto?> CloseTableAsync(string tableCode, string? closedBy = null);
    }
}