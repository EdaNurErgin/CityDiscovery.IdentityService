using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialService.Infrastructure.Data.Configurations
{
    public class PostLikeConfiguration : IEntityTypeConfiguration<PostLike>
    {
        public void Configure(EntityTypeBuilder<PostLike> builder)
        {
            builder.ToTable("PostLikes");

            builder.HasKey(l => l.Id); // BaseEntity'den gelen Id'yi anahtar olarak kullanıyoruz.

            // Bir kullanıcının aynı gönderiyi tekrar beğenmesini engellemek için
            // PostId ve UserId kombinasyonunun benzersiz (unique) olmasını sağlıyoruz.
            builder.HasIndex(l => new { l.PostId, l.UserId }).IsUnique();
        }
    }
}
