using DbScriptManager.Domain.Entities;
using DbScriptManager.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using DbScriptManager.Persistence.Interfaces;

namespace DbScriptManager.Persistence.Repositories;

public class VersionRepository : IVersionRepository
{
    private readonly AppDbContext _context;

    public VersionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<DbVersion>> GetAllAsync()
    {
        return await _context.Versions
            .Include(v => v.Scripts)
            .OrderByDescending(v => v.CreatedDate)
            .ToListAsync();
    }

    public async Task<DbVersion> GetByIdAsync(int id)
    {
        return await _context.Versions
            .Include(v => v.Scripts)
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task AddAsync(DbVersion version)
    {
        await _context.Versions.AddAsync(version);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var version = await _context.Versions.FindAsync(id);

        if (version != null)
        {
            _context.Versions.Remove(version);
            await _context.SaveChangesAsync();
        }
    }

}