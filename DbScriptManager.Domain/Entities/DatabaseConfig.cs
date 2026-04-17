// DbScriptManager.Domain.Entities/DatabaseConfig.cs
namespace DbScriptManager.Domain.Entities
{
    public class DatabaseConfig
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;          // Örn: "PROD_HR", "TEST_ERP"
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public ICollection<DbScript> Scripts { get; set; } = new List<DbScript>();
    }
}