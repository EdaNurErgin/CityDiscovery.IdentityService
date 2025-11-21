using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using IdentityService.Domain.Entities;

namespace IdentityService.Infrastructure.Data.Configurations
{
    public class UserDeviceConfiguration : IEntityTypeConfiguration<UserDevice>
    {
        public void Configure(EntityTypeBuilder<UserDevice> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.DeviceId)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.HasOne(x => x.User)
                   .WithMany(u => u.UserDevices)
                   .HasForeignKey(x => x.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            // 🔒 Kullanıcı + cihaz kombinasyonu benzersiz olacak
            builder.HasIndex(x => new { x.UserId, x.DeviceId }).IsUnique();

        }
    }
}
