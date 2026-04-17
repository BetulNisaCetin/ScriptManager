
using DbScriptManager.Application.Interfaces;
using DbScriptManager.Domain.Entities;
using DbScriptManager.Persistence.Context;
using Microsoft.EntityFrameworkCore;
namespace DbScriptManager.Persistence.Repositories
{
    public class ScriptRepository : IScriptRepository
    {
        private readonly AppDbContext _context;
        public ScriptRepository(AppDbContext context)
        {
            _context = context;

        }
        public async Task<List<DbScript>> GetAllAsync()
        {
            return await _context.Scripts.Include(s => s.Version).ToListAsync();
        }

        public async Task<DbScript> GetByIdAsync(int id)
        {
            return await _context.Scripts.Include(s => s.Version)
                 .FirstOrDefaultAsync(s => s.Id == id) ?? throw new Exception("Script bulunamadı");
        }

        public async Task AddAsync(DbScript script)
        {
            await _context.Scripts.AddAsync(script);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var script = await _context.Scripts.FindAsync(id);
            if (script != null)
            {
                _context.Scripts.Remove(script);
                await _context.SaveChangesAsync();
            }
        }



        public async Task<List<DbScript>> GetByVersionIdAsync(int versionId)
        {
            return await _context.Scripts
                .Include(s => s.Version)
                .Where(s => s.VersionId == versionId).
                OrderByDescending(s => s.CreatedDate).ToListAsync();
        }

        public async Task UpdateAsync(DbScript script)
        {
            _context.Scripts.Update(script);
            await _context.SaveChangesAsync();
        }

    }
}
