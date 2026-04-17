using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbScriptManager.Domain.Entities;

namespace DbScriptManager.Application.Interfaces
{
    public interface IScriptRepository
    {
        Task<List<DbScript>> GetAllAsync();
        Task<DbScript> GetByIdAsync(int id);
        Task AddAsync (DbScript script);
        Task DeleteAsync (int id);
        Task<List<DbScript>> GetByVersionIdAsync(int versionId);
        Task UpdateAsync(DbScript script);
    }
}
