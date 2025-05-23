using Microsoft.EntityFrameworkCore;
using StreamingOffApp.Models;

namespace StreamingOffApp.Data
{
    public class StreamingContext : DbContext
    {
        public DbSet<StreamingOffer> StreamingOffers { get; set; }

        public StreamingContext(DbContextOptions<StreamingContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StreamingOffer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PlatformName).HasMaxLength(100);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.PlanDays).IsRequired();
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(500);
            });

        }
    }
}