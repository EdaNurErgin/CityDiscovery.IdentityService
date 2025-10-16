using Microsoft.EntityFrameworkCore;
using SocialService.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SocialService.Infrastructure.Data.Configurations
{
    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            // Tablo adını belirtelim
            builder.ToTable("Posts");

            // Primary Key
            builder.HasKey(p => p.Id);

            // Content alanı zorunlu olsun ve maksimum 1000 karakter olsun
            builder.Property(p => p.Content)
                .IsRequired()
                .HasMaxLength(1000);

            // İlişkiler
            // Bir Post'un birden çok Comment'i olabilir.
            builder.HasMany(p => p.Comments)
                .WithOne(c => c.Post) // Bir Comment'in bir Post'u olur.
                .HasForeignKey(c => c.PostId) // Yabancı anahtar PostId'dir.
                .OnDelete(DeleteBehavior.Cascade); // Post silinirse yorumlar da silinsin.

            // Beğeniler için de aynı ilişkiyi kuruyoruz
            builder.HasMany(p => p.Likes)
                .WithOne(l => l.Post)
                .HasForeignKey(l => l.PostId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
