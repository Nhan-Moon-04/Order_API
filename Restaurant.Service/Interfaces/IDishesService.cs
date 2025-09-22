using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Restaurant.Domain.Entities;
using Restaurant.Domain.DTOs;
namespace Restaurant.Service.Interfaces
{
    public interface IDishesService
    {
        Task<IEnumerable<DishesDto>> GetAllDishesAsync();
  

    }
}
