using Microsoft.EntityFrameworkCore;
using projet_wpf.Models;

namespace projet_wpf.DataAccess
{
    public class AppDbContext : DbContext
    {
        public DbSet<PhotoModel> Photos { get; set; }
        public DbSet<TagItem> Tags { get; set; }

        private static bool _created = false;

        public AppDbContext()
        {
            if (!_created)
            {
                Database.EnsureCreated();
                _created = true;
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=photos.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TagItem>()
                .HasOne(t => t.Photo)
                .WithMany(p => p.Tags)
                .HasForeignKey(t => t.PhotoModelId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
