using Microsoft.EntityFrameworkCore;

namespace valera3.Models
{
    public class ValeraDbContext : DbContext
    {
        public DbSet<Valera> Valeras { get; set; }
        public DbSet<User> Users { get; set; }

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
                
                // Настройка связи Valera с User
                entity.HasOne(v => v.User)
                      .WithMany(u => u.Valeras)
                      .HasForeignKey(v => v.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(256);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.Email).IsUnique();
            });
        }
    }
}
