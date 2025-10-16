

using Microsoft.EntityFrameworkCore;
using SocialService.Domain.Entities;
using System.Reflection;

namespace SocialService.Infrastructure.Data
{
    public class SocialDbContext : DbContext
    {
        public SocialDbContext(DbContextOptions<SocialDbContext> options) : base(options)
        {
        }

        public DbSet<Post> Posts { get; set; }
        public DbSet<PostComment> PostComments { get; set; }
        public DbSet<PostLike> PostLikes { get; set; }
        public DbSet<PostPhoto> PostPhotos { get; set; }
        public DbSet<PostSaved> PostSaveds { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Bu satır, bu projede (Infrastructure) tanımlanmış tüm
            // IEntityTypeConfiguration sınıflarını otomatik olarak bulur ve uygular.
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);
        }
    }
}
