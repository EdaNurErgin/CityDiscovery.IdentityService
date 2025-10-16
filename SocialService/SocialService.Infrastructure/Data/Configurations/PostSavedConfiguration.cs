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
    public class PostSavedConfiguration : IEntityTypeConfiguration<PostSaved>
    {
        public void Configure(EntityTypeBuilder<PostSaved> builder)
        {
            builder.ToTable("PostSaveds");

            builder.HasKey(s => s.Id);

            // Bir kullanıcının aynı gönderiyi tekrar kaydetmesini engellemek için
            // PostId ve UserId kombinasyonunu benzersiz yapıyoruz.
            builder.HasIndex(s => new { s.PostId, s.UserId }).IsUnique();
        }
    }
}
