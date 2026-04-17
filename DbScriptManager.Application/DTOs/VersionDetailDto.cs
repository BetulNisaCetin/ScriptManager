using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbScriptManager.Application.DTOs
{
    public class VersionDetailDto
    {
        public int Id { get; set; }
        public string VersionName { get; set; }
        public DateTime CreatedDate { get; set; }

        public int ScriptCount { get; set; }

        public int RollbackCount { get; set; }

        public List<ScriptDto> Scripts { get; set; } = new();

        // Conflict Detection
        public List<ConflictInfo> AllConflicts { get; set; } = new();
        public int ConflictCount => AllConflicts.Count;
    }
}
