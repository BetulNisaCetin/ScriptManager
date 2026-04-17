using DbScriptManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace DbScriptManager.Persistence.Context
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<DbScript> Scripts { get; set; }
        public DbSet<DbVersion> Versions { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<DbVersion>()
                .HasKey(v => v.Id);
            builder.Entity<DbVersion>()
                .Property(v => v.VersionName)
                .IsRequired()
                .HasMaxLength(50);
            builder.Entity<DbScript>()
                .HasKey(s => s.Id);
            builder.Entity<DbScript>()
                .Property(s => s.ScriptName)
                .IsRequired()
                .HasMaxLength(200);
                

                
            builder.Entity<DbVersion>()
                .HasMany(v => v.Scripts)
                .WithOne(s => s.Version)
                .HasForeignKey(s => s.VersionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<DbScript>()
                .HasOne(s => s.CreatedByUser)
                .WithMany()
                .HasForeignKey(s => s.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
      }
    }
}
