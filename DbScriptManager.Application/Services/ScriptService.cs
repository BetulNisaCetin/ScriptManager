using DbScriptManager.Application.DTOs;
using DbScriptManager.Application.Interfaces;
using DbScriptManager.Domain.Entities;
using DbScriptManager.Persistence.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using DbScriptManager.Application.Helpers;
namespace DbScriptManager.Application.Services
{
    public class ScriptService : IScriptService
    {
        private readonly IScriptRepository _repository;
        private readonly IVersionRepository _versionRepository;
        private readonly IConfiguration _configuration;

    public ScriptService (
        IScriptRepository repository,
        IVersionRepository versionRepository,
        IConfiguration configuration
        )
        {
            _repository = repository;
            _versionRepository = versionRepository;
            _configuration = configuration;
        }

        public async Task<List<ScriptDto>> GetAllScripts()// içerik dosyadan file pathden geliyor
        {
            var scripts = await _repository.GetAllAsync();

            return scripts.Select(s => new ScriptDto
            {
                Id = s.Id,
                ScriptName = s.ScriptName,
                ScriptPath = s.ScriptPath,
                RollbackPath = s.RollbackPath,
                CreatedDate = s.CreatedDate,
                VersionName = s.Version?.VersionName?? "no version",
                IsExecuted = s.IsExecuted,
                ExecutedAt = s.ExecutedAt,
                IsSuccess = s.IsSuccess,
                ErrorMessagge = s.ErrorMessage,
                DeveloperName = s.DeveloperName,
                CreatedByUserId = s.CreatedByUserId,
                ScriptType = s.ScriptType ?? "Script",
            }).ToList();
        }

        public async Task CreateScript(CreateScriptDto dto)
        {
            var version = await _versionRepository.GetByIdAsync(dto.VersionId);
            if (version == null)
                throw new Exception("Versiyon bulunamadı");

            var rootPath = _configuration["ScriptSettings:RootPath"];
            if (string.IsNullOrWhiteSpace(rootPath))
                throw new Exception("Script root path tanımlı değil");

            // ScriptType doğrula: Cache | Script | Rollback
            var allowedTypes = new[] { "Cache", "Script", "Rollback" };
            var scriptType = allowedTypes.Contains(dto.ScriptType) ? dto.ScriptType : "Script";

            // Klasör yapısı: RootPath / VersionName / ScriptType /
            var typeFolder = Path.Combine(rootPath, version.VersionName, scriptType);
            if (!Directory.Exists(typeFolder))
                Directory.CreateDirectory(typeFolder);

            // Path traversal koruması
            var safeName = System.Text.RegularExpressions.Regex.Replace(
                dto.ScriptName, @"[^a-zA-Z0-9_\-]", "_");
            if (string.IsNullOrWhiteSpace(safeName))
                safeName = "script";

            var scriptPath  = Path.Combine(typeFolder, $"{safeName}.txt");
            var rollbackPath = Path.Combine(typeFolder, $"{safeName}_Rollback.txt");

            await File.WriteAllTextAsync(scriptPath, dto.ScriptContent);
            await File.WriteAllTextAsync(rollbackPath, dto.RollbackScript ?? string.Empty);

            var script = new DbScript
            {
                ScriptName       = dto.ScriptName,
                ScriptPath       = scriptPath,
                RollbackPath     = rollbackPath,
                VersionId        = version.Id,
                CreatedDate      = DateTime.Now,
                IsExecuted       = false,
                IsSuccess        = false,
                DeveloperName    = dto.DeveloperName,
                CreatedByUserId  = dto.CreatedByUserId ?? string.Empty,
                ScriptType       = scriptType
            };

            await _repository.AddAsync(script);
        }

        public async Task DeleteScript(int id)
        {
            var script = await _repository.GetByIdAsync(id);
            if (script == null)
                throw new Exception("Script bulunamadı");

            if (File.Exists(script.ScriptPath))
                File.Delete(script.ScriptPath);

            if (File.Exists(script.RollbackPath))
                File.Delete(script.RollbackPath);

            await _repository.DeleteAsync(id);
        }

        public async Task<List<ScriptDto>> GetScriptsByVersion(int versionId)
        {
            var scripts = await _repository.GetByVersionIdAsync(versionId);

            return scripts.Select(s => new ScriptDto
            {
                Id = s.Id,
                ScriptName = s.ScriptName,
                ScriptPath = s.ScriptPath,
                RollbackPath= s.RollbackPath,
                CreatedDate = s.CreatedDate,
                VersionName = s.Version?.VersionName ?? "no version",
                IsExecuted= s.IsExecuted,
                ExecutedAt =s.ExecutedAt,
                IsSuccess = s.IsSuccess,
                ErrorMessagge = s.ErrorMessage,
                DeveloperName = s.DeveloperName,
                CreatedByUserId = s.CreatedByUserId,
                ScriptType = s.ScriptType ?? "Script",
            }).ToList();
        }
        public async Task ExecuteScriptAsync(int id) //Burada set ediliyor
        {
            var script = await _repository.GetByIdAsync(id);

            if (script == null)
                throw new Exception("script bulunamadı");
            if (!File.Exists(script.ScriptPath))
                throw new Exception("script dosyası bulunamadı");
            var sql = await File.ReadAllTextAsync(script.ScriptPath);

            try
            {
                using var connection = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync();
                using var command = new SqlCommand(sql, connection);
                await command.ExecuteNonQueryAsync();

                script.IsExecuted = true;
                script.ExecutedAt = DateTime.Now;
                script.IsSuccess = true;
                script.ErrorMessage = null;
            }
            catch (Exception ex)
            {
                script.IsExecuted = true;
                script.ExecutedAt = DateTime.Now;
                script.IsSuccess = false;
                script.ErrorMessage = ex.Message;
            }
            await _repository.UpdateAsync(script);
        }
        public async Task<ScriptDetailDto> GetScriptDetail(int id)
        {
            var script = await _repository.GetByIdAsync(id);
            if (script == null)
                throw new Exception("Script bulunamadı");

            string scriptContent = string.Empty;
            string rollbackContent = string.Empty;

            if (!string.IsNullOrWhiteSpace(script.RollbackPath) && File.Exists(script.RollbackPath))
                rollbackContent = await File.ReadAllTextAsync(script.RollbackPath);
            if (!string.IsNullOrWhiteSpace(script.ScriptPath) && File.Exists(script.ScriptPath))
                scriptContent = await File.ReadAllTextAsync(script.ScriptPath);

            var operationType = SqlParserHelper.GetOperationSummary(scriptContent);
            var targetTables = SqlParserHelper.GetTargetTables(scriptContent);
            var targetTableSummary = SqlParserHelper.GetTargetTableSummary(scriptContent);

            return new ScriptDetailDto
            {
                Id = script.Id,
                ScriptName = script.ScriptName,
                VersionName = script.Version?.VersionName ?? string.Empty,
                DeveloperName = script.DeveloperName,
                ScriptPath = script.ScriptPath,
                RollbackPath = script.RollbackPath,
                ScriptContent = scriptContent,
                RollbackContent = rollbackContent,
                CreatedDate = script.CreatedDate,
                IsExecuted = script.IsExecuted,
                ExecutedAt = script.ExecutedAt,
                IsSuccess = script.IsSuccess,
                ErrorMessagge = script.ErrorMessage,
                OperationType = operationType,
                TargetTables = targetTables,
                TargetTableSummary = targetTableSummary,
                ScriptType = script.ScriptType ?? "Script"

                



            };
        }

        public async Task<ScriptDto?> GetById(int id)
        {
            var script = await _repository.GetByIdAsync(id);
            if (script == null) return null;

            return new ScriptDto
            {
                Id = script.Id,
                ScriptName = script.ScriptName,
                ScriptPath = script.ScriptPath,
                RollbackPath = script.RollbackPath,
                CreatedDate = script.CreatedDate,
                VersionName = script.Version?.VersionName ?? "no version",
                IsExecuted = script.IsExecuted,
                ExecutedAt = script.ExecutedAt,
                IsSuccess = script.IsSuccess,
                ErrorMessagge = script.ErrorMessage,
                DeveloperName = script.DeveloperName,
                CreatedByUserId = script.CreatedByUserId,
                ScriptType = script.ScriptType ?? "Script"
            };
        }

        public async Task Delete(int id)
        {
            await DeleteScript(id);
        }
    }
        
}