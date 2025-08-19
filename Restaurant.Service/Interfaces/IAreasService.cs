using Restaurant.Domain.DTOs;

namespace Restaurant.Service.Interfaces
{
    public interface IAreasService
    {
        Task<IEnumerable<AreasDto>> GetAllAsync();
        Task<AreasDto?> GetByIdAsync(string id);
        Task<AreasDto> CreateAsync(AreasDto dto);
        // To avoid ENC0023, do not uncomment these methods unless the application is restarted.
        // Task<bool> UpdateAsync(int id, AreasDto dto);
        // Task<bool> DeleteAsync(int id);
    }
}
