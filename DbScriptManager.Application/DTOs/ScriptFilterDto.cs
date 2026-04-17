// DbScriptManager.Application/DTOs/ScriptFilterDto.cs
namespace DbScriptManager.Application.DTOs
{
    public class ScriptFilterDto
    {
        public string? SearchTerm { get; set; }
        public int? VersionId { get; set; }
        public int? DatabaseConfigId { get; set; }
        public string? ScriptType { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}