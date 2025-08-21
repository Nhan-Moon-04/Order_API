using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Restaurant.Domain.DTOs;

namespace Restaurant.Service.Interfaces
{
    public interface IDishesGroupService
    {
        Task<IEnumerable<DishGroupDto>> GetAllGroup();
    }
}