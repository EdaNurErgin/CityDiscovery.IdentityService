using System;
using IdentityService.Domain.Common;

namespace IdentityService.Domain.Entities
{
    public class UserDevice : BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string DeviceId { get; set; } = default!;
        public string? DeviceName { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; } = default!;
    }
}
