using Microsoft.EntityFrameworkCore;

namespace valera3.Models
{
    public class ValeraDbContext : DbContext
    {
        public DbSet<Valera> Valeras { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=valera.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Valera>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Health).IsRequired();
                entity.Property(e => e.Mana).IsRequired();
                entity.Property(e => e.Cheerfulness).IsRequired();
                entity.Property(e => e.Fatigue).IsRequired();
                entity.Property(e => e.Money).IsRequired();
            });
        }
    }
}