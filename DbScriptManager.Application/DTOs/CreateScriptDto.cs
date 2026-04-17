using System.ComponentModel.DataAnnotations;

namespace DbScriptManager.Application.DTOs
{
    public class CreateScriptDto
    {
        [Required]
        public string ScriptName { get; set; } = string.Empty;
        [Required]
        public string ScriptContent { get; set; } = string.Empty;
        [Required]
        public string? RollbackScript { get; set; } = string.Empty;
        public int VersionId { get; set; }
        public string DeveloperName { get; set; } = string.Empty;
        public string? CreatedByUserId { get; set; }
        public int DatabaseConfigId { get; set; }  // YENİ

        /// <summary>Cache | Script | Rollback</summary>
        [Required]
        public string ScriptType { get; set; } = "Script";
    }
}

