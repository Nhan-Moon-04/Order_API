using Microsoft.EntityFrameworkCore;
using Restaurant.Data;
using Restaurant.Domain.DTOs;
using Restaurant.Domain.Entities;
using Restaurant.Service.Interfaces;

namespace Restaurant.Service.Services
{
    public class TableService : ITableService
    {
        private readonly RestaurantDbContext _context;

        public TableService(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TableDto>> GetAllTablesAsync()
        {
            var tables = await _context.Tables
                .Include(t => t.Area)
                .ToListAsync();

            return tables.Select(table => new TableDto
            {
                TableCode = table.TableCode,
                TableName = table.TableName,
                Capacity = table.Capacity,
                IsActive = table.IsActive,
                AreaId = table.AreaId,
                AreaName = table.Area?.AreaName
            });
        }

        public async Task<TableDto?> GetTableByIdAsync(string id)
        {
            var table = await _context.Tables
                .Include(t => t.Area)
                .FirstOrDefaultAsync(t => t.TableCode == id);

            return table == null ? null : new TableDto
            {
                TableCode = table.TableCode,
                TableName = table.TableName,
                Capacity = table.Capacity,
                IsActive = table.IsActive,
                AreaId = table.AreaId,
                AreaName = table.Area?.AreaName
            };
        }

        public async Task<IEnumerable<TableDto>> GetTablesByAreaIdAsync(string areaId)
        {
            var tables = await _context.Tables
                .Include(t => t.Area)
                .Where(t => t.AreaId == areaId)
                .ToListAsync();

            return tables.Select(table => new TableDto
            {
                TableCode = table.TableCode,
                TableName = table.TableName,
                Capacity = table.Capacity,
                IsActive = table.IsActive,
                AreaId = table.AreaId,
                AreaName = table.Area?.AreaName
            });
        }

        public async Task<TableDto> CreateTableAsync(TableDto dto)
        {
            var entity = new Table
            {
                TableCode = dto.TableCode,
                TableName = dto.TableName,
                Capacity = dto.Capacity,
                IsActive = dto.IsActive,
                AreaId = dto.AreaId
            };

            _context.Tables.Add(entity);
            await _context.SaveChangesAsync();

            return dto;
        }

        public async Task<TableDto?> UpdateTableAsync(string id, TableDto dto)
        {
            var entity = await _context.Tables.FirstOrDefaultAsync(t => t.TableCode == id);
            if (entity == null) return null;

            entity.TableName = dto.TableName;
            entity.Capacity = dto.Capacity;
            entity.IsActive = dto.IsActive;
            entity.AreaId = dto.AreaId;

            await _context.SaveChangesAsync();
            return dto;
        }

        public async Task<bool> DeleteTableAsync(string id)
        {
            var entity = await _context.Tables.FirstOrDefaultAsync(t => t.TableCode == id);
            if (entity == null) return false;

            _context.Tables.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<TableDto>> GetAvailableTablesAsync()
        {
            var tables = await _context.Tables
                .Include(t => t.Area)
                .Where(t => t.IsActive)
                .ToListAsync();

            return tables.Select(table => new TableDto
            {
                TableCode = table.TableCode,
                TableName = table.TableName,
                Capacity = table.Capacity,
                IsActive = table.IsActive,
                AreaId = table.AreaId,
                AreaName = table.Area?.AreaName
            });
        }
    }
}