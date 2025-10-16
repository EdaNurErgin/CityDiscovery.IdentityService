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
    public class PostCommentConfiguration : IEntityTypeConfiguration<PostComment>
    {
        public void Configure(EntityTypeBuilder<PostComment> builder)
        {
            builder.ToTable("PostComments");

            builder.HasKey(c => c.Id);

            // Yorum içeriği boş olamaz ve en fazla 500 karakter olabilir.
            builder.Property(c => c.Content)
                .IsRequired()
                .HasMaxLength(500);

            // PostId alanı zorunludur, bu bir yorumun mutlaka bir posta bağlı olması gerektiği anlamına gelir.
            // Bu ilişki zaten PostConfiguration'da kuruldu, ancak burada tekrar belirtmekte bir sakınca yok.
            builder.HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId);
        }
    }
}
