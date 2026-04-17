using DbScriptManager.Application.DTOs;
namespace DbScriptManager.WebUI.Models

{
    public class VersionDetailViewModel
    { public int Id { get; set; }
      public string VersionName { get; set; }
      public DateTime CreatedDate { get; set; }

      public int ScriptCount { get; set; }
      public int RollbackCount { get; set; }

       public List<ScriptDto> Scripts { get; set; } = new();
    }
}
