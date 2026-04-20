// DbScriptManager.Application/DTOs/CreateScriptDto.cs
using System.ComponentModel.DataAnnotations;

namespace DbScriptManager.Application.DTOs
{
    public class CreateScriptDto
    {
        [Required]
        public string ScriptName { get; set; } = string.Empty;

        public string? ScriptContent { get; set; }    // Required kaldırıldı

        public string? RollbackScript { get; set; }   // Required kaldırıldı

        public int VersionId { get; set; }
        public string DeveloperName { get; set; } = string.Empty;
        public string? CreatedByUserId { get; set; }
        public int DatabaseConfigId { get; set; }

        [Required]
        public string ScriptType { get; set; } = "Script";
    }
}