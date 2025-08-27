using Restaurant.Domain.DTOs;

namespace Restaurant.Service.Interfaces
{
    public interface IAreasService
    {
        Task<IEnumerable<AreasDto>> GetAllAsync();
        Task<AreasDto?> GetByIdAsync(string id);
        Task<AreasDto> CreateAsync(AreasDto dto);

        Task<int> CountAresa();


    }
}
