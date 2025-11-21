using System;
using IdentityService.Domain.Common;

namespace IdentityService.Domain.Entities
{
    public class RefreshToken : BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Token { get; set; } = default!;
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; } = false;

        // Kim adına verildi?
        public Guid UserId { get; set; }
        public User User { get; set; } = default!;

        // Hangi cihaza verildi? (kritik ek)
        public Guid UserDeviceId { get; set; }
        public UserDevice UserDevice { get; set; } = default!;
    }
}
