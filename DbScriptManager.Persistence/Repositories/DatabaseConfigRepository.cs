// DbScriptManager.Persistence/Repositories/DatabaseConfigRepository.cs
using DbScriptManager.Application.Interfaces;
using DbScriptManager.Domain.Entities;
using DbScriptManager.Persistence.Context;
using Microsoft.EntityFrameworkCore;


namespace DbScriptManager.Persistence.Repositories
{
    public class DatabaseConfigRepository : IDatabaseConfigRepository
    {
        private readonly AppDbContext _context;
        public DatabaseConfigRepository(AppDbContext context) => _context = context;

        public async Task<List<DatabaseConfig>> GetAllAsync()
            => await _context.DatabaseConfigs.ToListAsync();

        public async Task<DatabaseConfig?> GetByIdAsync(int id)
            => await _context.DatabaseConfigs.FindAsync(id);

        // DatabaseConfigRepository.cs — AddAsync metodunu güncelle
    public async Task AddAsync(DatabaseConfig config)       
    {
    var exists = await _context.DatabaseConfigs
        .AnyAsync(d => d.Name.ToLower() == config.Name.ToLower());

    if (exists)
        throw new Exception($"'{config.Name}' adında bir database zaten mevcut");

    _context.DatabaseConfigs.Add(config);
    await _context.SaveChangesAsync();
    }

        public async Task DeleteAsync(int id)
        {
            var config = await _context.DatabaseConfigs.FindAsync(id);
            if (config != null)
            {
                _context.DatabaseConfigs.Remove(config);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> HasScriptsAsync(int id)
            => await _context.Scripts.AnyAsync(s => s.DatabaseConfigId == id);
    }
}