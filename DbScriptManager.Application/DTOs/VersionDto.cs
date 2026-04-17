using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbScriptManager.Application.DTOs
{
    public class VersionDto
    {
        public int Id { get; set; }
        public string VersionName { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ScriptCount { get; set; }
        public int RollbackCount { get; set; }

    }
}
