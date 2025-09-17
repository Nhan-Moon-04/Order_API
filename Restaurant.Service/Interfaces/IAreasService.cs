using Restaurant.Domain.DTOs;

namespace Restaurant.Service.Interfaces
{
    public interface IAreasService
    {
        Task<IEnumerable<AreasDto>> GetAllAsync();
        Task<AreasDto?> GetByIdAsync(string id);

        Task<int> CountAresa();

        Task<bool> UpdateAsync(string id, AreasDto dto);


    }
}
