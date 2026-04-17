using DbScriptManager.Application.DTOs;

namespace DbScriptManager.Application.Interfaces
{
    public interface IConflictDetectionService
    {
        /// <summary>
        /// Verilen script listesindeki çakışmaları tespit eder.
        /// Her script için dosyadan SQL içeriğini okur.
        /// </summary>
        Task<List<ConflictInfo>> DetectConflictsAsync(List<ScriptDto> scripts);
    }
}
