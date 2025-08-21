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

    }
}