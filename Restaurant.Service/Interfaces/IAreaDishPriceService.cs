using Restaurant.Domain.DTOs;

namespace Restaurant.Service.Interfaces
{
    public interface IAreaDishPriceService
    {
        Task<IEnumerable<AreaDishPriceDto>> GetAllAsync();
        Task<IEnumerable<AreaDishPriceDto>> GetByIdAsync(string areaId);

        Task<bool> UpdateAsync(string id, AreaDishPriceDto dto);


        Task<AreaDishPriceDto> UpdatePriceAsync(string id, decimal customPrice);
    }
}
