using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbScriptManager.Domain.Entities
{
    public class DbScript
    { 
        public int Id { get; set; }
        public int VersionId { get; set; }
        public string ScriptName { get; set; } = string.Empty;
        public string ScriptPath { get; set; } = string.Empty;
        public string RollbackPath { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DbVersion Version { get; set; } = null;
        public bool IsExecuted { get; set; }
        public DateTime? ExecutedAt { get; set; }
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public string DeveloperName { get; set; }
        public string CreatedByUserId { get; set; } = string.Empty;
        public AppUser CreatedByUser { get; set; }

        /// <summary>Cache | Script | Rollback</summary>
        public string ScriptType { get; set; } = "Script";
    }
}
