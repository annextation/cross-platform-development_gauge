using Cross_platform_development_technologies.Models;
using Microsoft.EntityFrameworkCore;

namespace Cross_platform_development_technologies.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Gauge> Gauges => Set<Gauge>();
        public DbSet<AppUser> Users => Set<AppUser>();

        public DbSet<ObservationSession> Sessions
            => Set<ObservationSession>();

        public DbSet<Measurement> Measurements
            => Set<Measurement>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AppUser>()
                .HasIndex(user => user.Login)
                .IsUnique();

            modelBuilder.Entity<ObservationSession>()
                .HasOne<Gauge>()
                .WithMany()
                .HasForeignKey(session => session.GaugeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Measurement>()
                .HasOne<ObservationSession>()
                .WithMany()
                .HasForeignKey(measurement => measurement.SessionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}