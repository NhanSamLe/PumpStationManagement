using Microsoft.EntityFrameworkCore;
using PumpStationManagement_API.Models;

namespace PumpStationManagement_API.Services
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public virtual DbSet<Alert> Alerts { get; set; }

        public virtual DbSet<MaintenanceHistory> MaintenanceHistories { get; set; }

        public virtual DbSet<OperatingData> OperatingDatas { get; set; }

        public virtual DbSet<Pump> Pumps { get; set; }

        public virtual DbSet<PumpStation> PumpStations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình mối quan hệ cho Alert.CreatedByNavigation
            modelBuilder.Entity<Alert>()
                .HasOne(a => a.CreatedByNavigation)
                .WithMany(u => u.AlertCreatedByNavigations)
                .HasForeignKey(a => a.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict); // Hoặc DeleteBehavior.SetNull nếu muốn cho phép null

            // Cấu hình mối quan hệ cho Alert.ModifiedByNavigation
            modelBuilder.Entity<Alert>()
                .HasOne(a => a.ModifiedByNavigation)
                .WithMany(u => u.AlertModifiedByNavigations)
                .HasForeignKey(a => a.ModifiedBy)
                .OnDelete(DeleteBehavior.Restrict); // Hoặc DeleteBehavior.SetNull nếu muốn cho phép null

            // Cấu hình mối quan hệ cho Pump và User
            modelBuilder.Entity<Pump>()
                .HasOne(p => p.CreatedByNavigation)
                .WithMany(u => u.PumpCreatedByNavigations)
                .HasForeignKey(p => p.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pump>()
                .HasOne(p => p.ModifiedByNavigation)
                .WithMany(u => u.PumpModifiedByNavigations)
                .HasForeignKey(p => p.ModifiedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình mối quan hệ cho PumpStation và User
            modelBuilder.Entity<PumpStation>()
                .HasOne(ps => ps.CreatedByNavigation)
                .WithMany(u => u.PumpStationCreatedByNavigations)
                .HasForeignKey(ps => ps.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PumpStation>()
                .HasOne(ps => ps.ModifiedByNavigation)
                .WithMany(u => u.PumpStationModifiedByNavigations)
                .HasForeignKey(ps => ps.ModifiedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình mối quan hệ cho MaintenanceHistory và User
            modelBuilder.Entity<MaintenanceHistory>()
                .HasOne(m => m.PerformedByNavigation)
                .WithMany(u => u.MaintenanceHistories)
                .HasForeignKey(m => m.PerformedBy)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
