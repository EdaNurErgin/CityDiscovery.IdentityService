using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using IdentityService.Domain.Entities;

namespace IdentityService.Infrastructure.Data.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Token)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.HasIndex(x => x.Token).IsUnique();
            builder.HasIndex(x => x.UserDeviceId).IsUnique(); // cihaz başına tek token

            // 🔻 BURASI ÖNEMLİ: User FK'sında Cascade YOK
            builder.HasOne(x => x.User)
                   .WithMany(u => u.RefreshTokens)
                   .HasForeignKey(x => x.UserId)
                   .OnDelete(DeleteBehavior.NoAction); // veya .Restrict

            // Cihaz FK'sında Cascade VAR
            builder.HasOne(x => x.UserDevice)
                   .WithMany()
                   .HasForeignKey(x => x.UserDeviceId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
