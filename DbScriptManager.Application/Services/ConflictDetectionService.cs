using DbScriptManager.Application.DTOs;
using DbScriptManager.Application.Helpers;
using DbScriptManager.Application.Interfaces;

namespace DbScriptManager.Application.Services
{
    public class ConflictDetectionService : IConflictDetectionService
    {
        public async Task<List<ConflictInfo>> DetectConflictsAsync(List<ScriptDto> scripts)
        {
            var conflicts = new List<ConflictInfo>();

            // Her script için SQL içeriğini oku
            var scriptContents = new Dictionary<int, (string Sql, List<string> Tables)>();

            foreach (var script in scripts)
            {
                var sql = string.Empty;
                if (!string.IsNullOrWhiteSpace(script.ScriptPath) && File.Exists(script.ScriptPath))
                {
                    sql = await File.ReadAllTextAsync(script.ScriptPath);
                }
                else if (!string.IsNullOrWhiteSpace(script.ScriptPath))
                {
                    // Dosya bulunamadı — bu script çakışma analizine dahil edilemez
                    conflicts.Add(new ConflictInfo
                    {
                        ScriptId = script.Id,
                        ScriptName = script.ScriptName,
                        ConflictType = "MissingFile",
                        Severity = "Medium",
                        Description = $"'{script.ScriptName}' script dosyası bulunamadı, analiz yapılamadı."
                    });
                }

                var tables = SqlParserHelper.GetTargetTables(sql);
                scriptContents[script.Id] = (sql, tables);
            }

            // 1. CACHE BUSTING tespiti — her script tek tek kontrol edilir
            foreach (var script in scripts)
            {
                var (sql, _) = scriptContents[script.Id];
                if (string.IsNullOrWhiteSpace(sql)) continue;

                var cacheBusting = SqlParserHelper.DetectCacheBusting(sql);
                if (!cacheBusting.Any()) continue;

                var highOps = cacheBusting.Where(c => c.Severity == "High").Select(c => c.Label).ToList();
                var medOps  = cacheBusting.Where(c => c.Severity == "Medium").Select(c => c.Label).ToList();
                var allOps  = cacheBusting.Select(c => c.Label).ToList();

                var severity = highOps.Any() ? "High" : "Medium";
                var desc = severity == "High"
                    ? $"Bu script plan cache'i temizleyen operasyon içeriyor: {string.Join(", ", highOps)}"
                    : $"Bu script cache'i etkileyen operasyon içeriyor: {string.Join(", ", medOps)}";

                conflicts.Add(new ConflictInfo
                {
                    ScriptId = script.Id,
                    ScriptName = script.ScriptName,
                    ConflictType = "CacheBusting",
                    CacheBustingOps = allOps,
                    Severity = severity,
                    Description = desc
                });
            }

            // 2. TABLO ÇAKIŞMASI tespiti — her script çifti karşılaştırılır
            var scriptList = scripts.ToList();
            for (int i = 0; i < scriptList.Count; i++)
            {
                for (int j = i + 1; j < scriptList.Count; j++)
                {
                    var scriptA = scriptList[i];
                    var scriptB = scriptList[j];

                    var tablesA = scriptContents[scriptA.Id].Tables;
                    var tablesB = scriptContents[scriptB.Id].Tables;

                    if (!tablesA.Any() || !tablesB.Any()) continue;

                    var sharedTables = tablesA
                        .Intersect(tablesB, StringComparer.OrdinalIgnoreCase)
                        .ToList();

                    foreach (var table in sharedTables)
                    {
                        // Her paylaşılan tablo için çakışma severity'sini belirle
                        var severity = DetermineTableConflictSeverity(
                            scriptContents[scriptA.Id].Sql,
                            scriptContents[scriptB.Id].Sql,
                            table);

                        conflicts.Add(new ConflictInfo
                        {
                            ScriptId = scriptA.Id,
                            ScriptName = scriptA.ScriptName,
                            ConflictingScriptId = scriptB.Id,
                            ConflictingScriptName = scriptB.ScriptName,
                            ConflictType = "TableConflict",
                            ConflictTable = table,
                            Severity = severity,
                            Description = $"'{scriptA.ScriptName}' ve '{scriptB.ScriptName}' aynı tabloyu etkiliyor: [{table}]"
                        });
                    }
                }
            }

            return conflicts;
        }

        /// <summary>
        /// İki scriptin aynı tabloyu nasıl etkilediğine göre severity belirler.
        /// Her ikisi de DDL yapıyorsa High, biri DDL biri DML ise High,
        /// ikisi de DML ise Medium.
        /// </summary>
        private static string DetermineTableConflictSeverity(string sqlA, string sqlB, string _)
        {
            var ddlKeywords = new[] { "ALTER TABLE", "DROP TABLE", "TRUNCATE TABLE", "CREATE TABLE" };

            bool aIsDdl = ddlKeywords.Any(k =>
                System.Text.RegularExpressions.Regex.IsMatch(sqlA, $@"\b{k}\b", System.Text.RegularExpressions.RegexOptions.IgnoreCase));
            bool bIsDdl = ddlKeywords.Any(k =>
                System.Text.RegularExpressions.Regex.IsMatch(sqlB, $@"\b{k}\b", System.Text.RegularExpressions.RegexOptions.IgnoreCase));

            if (aIsDdl || bIsDdl) return "High";
            return "Medium";
        }
    }
}
