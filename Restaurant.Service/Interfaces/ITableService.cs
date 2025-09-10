using Restaurant.Domain.DTOs;

namespace Restaurant.Service.Interfaces
{
    public interface ITableService
    {


        Task<TableDto?> OpenTableAsync(string tableCode, string areaId, string? openedBy = null);
        Task<TableDto?> CloseTableAsync(string tableCode, string? closedBy = null);
        Task<IEnumerable<TableDto>> GetAllTablesAsync();
    }
}