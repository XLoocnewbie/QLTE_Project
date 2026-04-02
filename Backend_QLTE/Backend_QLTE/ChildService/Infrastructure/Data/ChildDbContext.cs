using Microsoft.EntityFrameworkCore;
using Backend_QLTE.ChildService.Domain.Entities;

namespace Backend_QLTE.ChildService.Infrastructure.Data
{
    public class ChildDbContext : DbContext
    {
        public ChildDbContext(DbContextOptions<ChildDbContext> options) : base(options) { }

        public DbSet<Child> Children { get; set; }
        public DbSet<LocationHistory> LocationHistories { get; set; }
        public DbSet<SafeZone> SafeZones { get; set; }
        public DbSet<DangerZone> DangerZones { get; set; }
        public DbSet<Alert> Alerts { get; set; }
        public DbSet<SOSRequest> SOSRequests { get; set; }
        public DbSet<DeviceInfo> DeviceInfos { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<StudyPeriod> StudyPeriods { get; set; }
        public DbSet<Schedule> Schedules { get; set; }             
        public DbSet<ExamSchedule> ExamSchedules { get; set; }
        public DbSet<DeviceRestriction> DeviceRestrictions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Child>()
                .HasOne(c => c.DeviceInfo)
                .WithOne(d => d.Child)
                .HasForeignKey<DeviceInfo>(d => d.ChildId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Child>()
                .HasMany(c => c.LocationHistories)
                .WithOne(l => l.Child)
                .HasForeignKey(l => l.ChildId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Child>()
                .HasMany(c => c.StudyPeriods)
                .WithOne(sp => sp.Child)
                .HasForeignKey(sp => sp.ChildId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Child>()
                .HasMany(c => c.Schedules)
                .WithOne(s => s.Child)
                .HasForeignKey(s => s.ChildId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Child>()
                .HasMany(c => c.ExamSchedules)
                .WithOne(es => es.Child)
                .HasForeignKey(es => es.ChildId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DeviceInfo>()
                .HasMany(d => d.DeviceRestrictions)
                .WithOne(r => r.Device)
                .HasForeignKey(r => r.DeviceId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
