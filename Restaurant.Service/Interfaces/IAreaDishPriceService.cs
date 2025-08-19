using Restaurant.Domain.DTOs;

namespace Restaurant.Service.Interfaces
{
    public interface IAreaDishPriceService
    {
        Task<IEnumerable<AreaDishPriceDto>> GetAllAsync();
        Task<AreaDishPriceDto?> GetByIdAsync(string id);
        Task<AreaDishPriceDto> CreateAsync(AreaDishPriceDto dto);
        Task<bool> UpdateAsync(string id, AreaDishPriceDto dto);
        Task<bool> DeleteAsync(string id);
    }
}
