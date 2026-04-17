namespace DbScriptManager.Application.DTOs
{
    public class ConflictInfo
    {
        // Çakışmada birinci script
        public int ScriptId { get; set; }
        public string ScriptName { get; set; } = string.Empty;

        // Tablo çakışmasında ikinci script — CacheBusting için null
        public int? ConflictingScriptId { get; set; } = null;
        public string? ConflictingScriptName { get; set; }

        // "TableConflict" | "CacheBusting"
        public string ConflictType { get; set; } = string.Empty;

        // Tablo çakışmasında hangi tablo
        public string? ConflictTable { get; set; }

        // Cache bozma operasyonları
        public List<string> CacheBustingOps { get; set; } = new();

        // "High" | "Medium" | "Warning"
        public string Severity { get; set; } = "Warning";

        public string Description { get; set; } = string.Empty;
    }
}
