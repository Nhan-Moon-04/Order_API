using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Restaurant.Domain.Entities;
using Restaurant.Domain.DTOs;

namespace Restaurant.Service.Interfaces
{
    public interface IKitchensService
    {
        Task<IEnumerable<KitchensDto>> GetAllAsync();
        Task<KitchensDto?> GetByIdAsync(string id);
        Task<KitchensDto> CreateAsync(KitchensDto dto);
        Task<bool> UpdateAsync(string id, KitchensDto dto);
        Task<bool> DeleteAsync(string id);
    }
}
