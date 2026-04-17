using System.Text.RegularExpressions;

namespace DbScriptManager.Application.Helpers
{
    public static class SqlParserHelper
    {
        // =============================================
        // Cache-busting pattern tanımları
        // Severity: High = plan cache tamamen bozulur, Medium = tablo/index cache bozulur
        // =============================================
        private static readonly (string Pattern, string Label, string Severity)[] CacheBustingPatterns =
        {
            // HIGH — tüm plan cache'i temizler
            (@"\bDBCC\s+FREEPROCCACHE\b",       "DBCC FREEPROCCACHE",       "High"),
            (@"\bDBCC\s+DROPCLEANBUFFERS\b",    "DBCC DROPCLEANBUFFERS",    "High"),
            (@"\bDBCC\s+FREESYSTEMCACHE\b",     "DBCC FREESYSTEMCACHE",     "High"),
            (@"\bDBCC\s+FLUSHPROCINDB\b",       "DBCC FLUSHPROCINDB",       "High"),

            // HIGH — tablo/index yapısını tamamen değiştirir
            (@"\bDROP\s+TABLE\b",               "DROP TABLE",               "High"),
            (@"\bTRUNCATE\s+TABLE\b",           "TRUNCATE TABLE",           "High"),
            (@"\bDROP\s+INDEX\b",               "DROP INDEX",               "High"),

            // MEDIUM — istatistik veya recompile
            (@"\bUPDATE\s+STATISTICS\b",        "UPDATE STATISTICS",        "Medium"),
            (@"\bsp_recompile\b",               "sp_recompile",             "Medium"),
            (@"\bsp_updatestats\b",             "sp_updatestats",           "Medium"),
            (@"\bALTER\s+INDEX\b",              "ALTER INDEX",              "Medium"),
            (@"\bALTER\s+TABLE\b",              "ALTER TABLE",              "Medium"),

            // MEDIUM — index oluşturma/yeniden oluşturma
            (@"\bCREATE\s+(UNIQUE\s+)?(CLUSTERED\s+)?INDEX\b", "CREATE INDEX", "Medium"),
        };

        // =============================================
        // Operation type detection
        // =============================================
        public static List<string> GetOperationTypes(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                return new List<string>();

            var operations = new List<string>();
            var upperSql = sql.ToUpperInvariant();

            if (Regex.IsMatch(upperSql, @"\bUPDATE\b"))
                operations.Add("UPDATE");
            if (Regex.IsMatch(upperSql, @"\bINSERT\s+INTO\b"))
                operations.Add("INSERT");
            if (Regex.IsMatch(upperSql, @"\bALTER\s+TABLE\b"))
                operations.Add("ALTER TABLE");
            if (Regex.IsMatch(upperSql, @"\bCREATE\s+TABLE\b"))
                operations.Add("CREATE TABLE");
            if (Regex.IsMatch(upperSql, @"\bDELETE\s+FROM\b"))
                operations.Add("DELETE");
            if (Regex.IsMatch(upperSql, @"\bTRUNCATE\s+TABLE\b"))
                operations.Add("TRUNCATE");
            if (Regex.IsMatch(upperSql, @"\bDROP\s+TABLE\b"))
                operations.Add("DROP TABLE");
            if (Regex.IsMatch(upperSql, @"\bDROP\s+INDEX\b"))
                operations.Add("DROP INDEX");

            return operations.Distinct().ToList();
        }

        // =============================================
        // Table name extraction
        // =============================================
        public static List<string> GetTargetTables(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                return new List<string>();

            var tables = new List<string>();

            var patterns = new[]
            {
                @"\bUPDATE\s+\[?(?:\w+)\]?\.\[?(?<Table>\w+)\]?|\bUPDATE\s+\[?(?<Table>\w+)\]?",
                @"\bINSERT\s+INTO\s+\[?(?:\w+)\]?\.\[?(?<Table>\w+)\]?|\bINSERT\s+INTO\s+\[?(?<Table>\w+)\]?",
                @"\bALTER\s+TABLE\s+\[?(?:\w+)\]?\.\[?(?<Table>\w+)\]?|\bALTER\s+TABLE\s+\[?(?<Table>\w+)\]?",
                @"\bCREATE\s+TABLE\s+\[?(?:\w+)\]?\.\[?(?<Table>\w+)\]?|\bCREATE\s+TABLE\s+\[?(?<Table>\w+)\]?",
                @"\bDELETE\s+FROM\s+\[?(?:\w+)\]?\.\[?(?<Table>\w+)\]?|\bDELETE\s+FROM\s+\[?(?<Table>\w+)\]?",
                @"\bTRUNCATE\s+TABLE\s+\[?(?:\w+)\]?\.\[?(?<Table>\w+)\]?|\bTRUNCATE\s+TABLE\s+\[?(?<Table>\w+)\]?",
                @"\bDROP\s+TABLE\s+\[?(?:\w+)\]?\.\[?(?<Table>\w+)\]?|\bDROP\s+TABLE\s+\[?(?<Table>\w+)\]?",
            };

            foreach (var pattern in patterns)
            {
                var matches = Regex.Matches(sql, pattern, RegexOptions.IgnoreCase);
                foreach (Match match in matches)
                {
                    if (match.Groups["Table"].Success)
                        tables.Add(match.Groups["Table"].Value);
                }
            }

            return tables
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        // =============================================
        // Cache-busting detection
        // Returns: list of (label, severity) tuples
        // =============================================
        public static List<(string Label, string Severity)> DetectCacheBusting(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                return new List<(string, string)>();

            var found = new List<(string Label, string Severity)>();

            foreach (var (pattern, label, severity) in CacheBustingPatterns)
            {
                if (Regex.IsMatch(sql, pattern, RegexOptions.IgnoreCase))
                    found.Add((label, severity));
            }

            return found;
        }

        // =============================================
        // Summary helpers
        // =============================================
        public static string GetOperationSummary(string sql)
        {
            var operations = GetOperationTypes(sql);
            return operations.Any() ? string.Join(", ", operations) : "Bilinmiyor";
        }

        public static string GetTargetTableSummary(string sql)
        {
            var tables = GetTargetTables(sql);
            return tables.Any() ? string.Join(", ", tables) : "-";
        }
    }
}
