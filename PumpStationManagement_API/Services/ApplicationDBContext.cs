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

        public virtual DbSet<AuditLog> AuditLogs { get; set; }
        public virtual DbSet<PumpStatusBackup> PumpStatusBackups { get; set; }
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
            // OperatingData - CreatedBy
            modelBuilder.Entity<OperatingData>()
                .HasOne(od => od.CreatedByNavigation)
                .WithMany(u => u.OperatingCreatedByNavigations)
                .HasForeignKey(od => od.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // OperatingData - ModifiedBy
            modelBuilder.Entity<OperatingData>()
                .HasOne(od => od.ModifiedByNavigation)
                .WithMany(u => u.OperatingModifiedByNavigations)
                .HasForeignKey(od => od.ModifiedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // MaintenanceHistory - CreatedBy
            modelBuilder.Entity<MaintenanceHistory>()
                .HasOne(m => m.CreatedByNavigation)
                .WithMany(u => u.MaintenanceCreatedByNavigations)
                .HasForeignKey(m => m.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // MaintenanceHistory - ModifiedBy
            modelBuilder.Entity<MaintenanceHistory>()
                .HasOne(m => m.ModifiedByNavigation)
                .WithMany(u => u.MaintenanceModifiedByNavigations)
                .HasForeignKey(m => m.ModifiedBy)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<PumpStatusBackup>()
                .HasOne(b => b.Pump)
                .WithOne() // hoặc .WithMany() nếu bạn cho phép một pump có nhiều backup
                .HasForeignKey<PumpStatusBackup>(b => b.PumpId)
                .OnDelete(DeleteBehavior.Restrict); // hoặc .SetNull nếu cần

        }

    }
}
