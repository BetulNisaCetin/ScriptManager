using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbScriptManager.Application.DTOs
{
    public class ScriptDto
    {
        public int Id { get; set; }
        public string ScriptName { get; set; } = string.Empty;
        public string ScriptPath { get; set; } = string.Empty;
        public string RollbackPath { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string VersionName { get; set; } = string.Empty;
        public bool IsExecuted { get; set; } 
        public DateTime? ExecutedAt { get; set; }
        public bool IsSuccess { get; set; }
        public string? ErrorMessagge { get; set; }
        public string DeveloperName { get; set; }
        public string? CreatedByUserId { get; set; }

        /// <summary>Cache | Script | Rollback</summary>
        public string ScriptType { get; set; } = "Script";

        // Conflict Detection
        public bool HasConflict { get; set; }
        public string? ConflictSeverity { get; set; }
        public List<ConflictInfo> Conflicts { get; set; } = new();
    }
}

