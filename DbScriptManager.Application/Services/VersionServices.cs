using DbScriptManager.Application.DTOs;
using DbScriptManager.Application.Interfaces;
using DbScriptManager.Domain.Entities;
using DbScriptManager.Persistence.Interfaces;

namespace DbScriptManager.Application.Services;

public class VersionService : IVersionService
{
    private readonly IVersionRepository _versionRepository;
    private readonly IConflictDetectionService _conflictDetectionService;

    public VersionService(
        IVersionRepository versionRepository,
        IConflictDetectionService conflictDetectionService)
    {
        _versionRepository = versionRepository;
        _conflictDetectionService = conflictDetectionService;
    }

    public async Task CreateVersion(CreateVersionDto dto)//versiyon oluşturma
    {
        var entity = new DbVersion
        {
            VersionName = dto.VersionName,
            CreatedDate = DateTime.Now,
        };
        await _versionRepository.AddAsync(entity);
    }

    public async Task DeleteVersion(int id)
    {
        var version = await _versionRepository.GetByIdAsync(id); //ilgili versiyon çekilir

        if (version == null)
            throw new Exception("versiyon bulunamadı");
        if (version.Scripts != null && version.Scripts.Any())
            throw new Exception("Bu versiyona ait scriptler bulunduğu için silinemez");
        await _versionRepository.DeleteAsync(id);
        
    }

    public async Task<List<VersionDto>> GetAllVersions()
    {
        var versions = await _versionRepository.GetAllAsync();
        //EN YENİ EN ÜSTTE 
        return versions.OrderByDescending(v => v.CreatedDate).Select(v => new VersionDto
        {
            Id = v.Id,
            VersionName = v.VersionName,
            CreatedDate = v.CreatedDate,
            ScriptCount = v.Scripts != null ? v.Scripts.Count : 0,
            RollbackCount = v.Scripts != null
            ? v.Scripts.Count(s => !string.IsNullOrEmpty(s.RollbackPath))
            : 0
        }).ToList();
            
    }
    public async Task DeleteVersions(List<int> ids)// her seçilen versiyonu kontrol eder , script varsa exception yoksa siler.
    {
        foreach(var id in ids)
        {
            var version = await _versionRepository.GetByIdAsync(id);

            if (version == null)
                continue;
            if (version.Scripts != null && version.Scripts.Any())
                throw new Exception($"'{version.VersionName}' versiyonuna ait scriptler bulunduğu için silinemez");

            await _versionRepository.DeleteAsync(id);
        }
    }

    public async Task<VersionDetailDto> GetVersionDetail(int id)
    {
        var version = await _versionRepository.GetByIdAsync(id);

        if (version == null)
            throw new Exception("Versiyon bulunamadı");

        var scripts = version.Scripts != null
            ? version.Scripts
                .OrderByDescending(s => s.CreatedDate)
                .Select(s => new ScriptDto
                {
                    Id = s.Id,
                    ScriptName = s.ScriptName,
                    DeveloperName = s.DeveloperName,
                    ScriptPath = s.ScriptPath,
                    RollbackPath = s.RollbackPath,
                    CreatedDate = s.CreatedDate,
                    VersionName = s.Version?.VersionName ?? string.Empty,
                    CreatedByUserId = s.CreatedByUserId
                }).ToList()
            : new List<ScriptDto>();

        // Conflict detection
        var allConflicts = await _conflictDetectionService.DetectConflictsAsync(scripts);

        // Her scripte kendi çakışmalarını ata
        foreach (var script in scripts)
        {
            var scriptConflicts = allConflicts
                .Where(c => c.ScriptId == script.Id || c.ConflictingScriptId == script.Id)
                .ToList();

            script.Conflicts = scriptConflicts;
            script.HasConflict = scriptConflicts.Any();
            script.ConflictSeverity = scriptConflicts.Any()
                ? (scriptConflicts.Any(c => c.Severity == "High") ? "High" : "Medium")
                : null;
        }

        return new VersionDetailDto
        {
            Id = version.Id,
            VersionName = version.VersionName,
            CreatedDate = version.CreatedDate,
            ScriptCount = scripts.Count,
            RollbackCount = scripts.Count(s => !string.IsNullOrWhiteSpace(s.RollbackPath)),
            Scripts = scripts,
            AllConflicts = allConflicts
        };
    }
}