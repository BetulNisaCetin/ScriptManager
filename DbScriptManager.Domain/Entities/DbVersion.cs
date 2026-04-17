using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbScriptManager.Domain.Entities
{
    public class DbVersion
    {
        public int Id { get; set; }
        public string VersionName { get; set; }
        public DateTime CreatedDate { get; set; }
        public ICollection<DbScript> Scripts { get; set; } = new List<DbScript>();
    }
}
