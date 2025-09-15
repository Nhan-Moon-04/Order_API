using Restaurant.Domain.DTOs;
using Restaurant.Domain.DTOs.Query;
using Restaurant.Domain.DTOs.Request;

namespace Restaurant.Service.Interfaces
{
    public interface IAreaDishPriceService
    {
        Task AddDishesToAreaAsync(AddAreaDishPriceRequest request);
        Task<bool> DeleteAsync(string id);
        Task<IEnumerable<AreaDishPriceDto>> GetAllAsync();
        Task<IEnumerable<AreaDishPriceDto>> GetByIdAsync(string areaId);
        Task<(IEnumerable<AreaDishPriceDto> Items, int TotalRecords)> GetPagedAreaDishPriceAsync(AreaDishPriceQueryParameters query);
        Task<bool> UpdateAsync(string id, AreaDishPriceDto dto);


        Task<AreaDishPriceDto> UpdatePriceAsync(string id, decimal customPrice);
    }
}
