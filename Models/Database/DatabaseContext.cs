using Microsoft.EntityFrameworkCore;
using EvacuationPlanning.Models;

namespace CourseService.Models.Database
{
    public class DatabaseContext : DbContext
    {
        public DbSet<TableVehicle> TableVehicles { get; set; }
        public DbSet<TableEvacuationZone> TableEvacuationZone { get; set; }

        public DatabaseContext() { }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TableVehicle>()
                .Property(o => o.Type)
                .HasConversion<string>();
        }
    }
}