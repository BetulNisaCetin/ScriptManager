
using DbScriptManager.Domain.Entities;

namespace DbScriptManager.Application.Interfaces
{
    public interface IDatabaseConfigRepository
    {
        Task<List<DatabaseConfig>> GetAllAsync();
        Task<DatabaseConfig?> GetByIdAsync(int id);
        Task AddAsync(DatabaseConfig config);
        Task DeleteAsync(int id);
        Task<bool> HasScriptsAsync(int id);
    }
}