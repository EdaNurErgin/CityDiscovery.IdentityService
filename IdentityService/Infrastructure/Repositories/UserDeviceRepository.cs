using IdentityService.Application.Interfaces;
using IdentityService.Domain.Entities;
using IdentityService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Infrastructure.Repositories
{
    public class UserDeviceRepository : IUserDeviceRepository
    {
        private readonly IdentityDbContext _db;
        public UserDeviceRepository(IdentityDbContext db) => _db = db;

        public Task<UserDevice?> GetByUserAndDeviceAsync(Guid userId, string deviceId)
            => _db.UserDevices.FirstOrDefaultAsync(d => d.UserId == userId && d.DeviceId == deviceId);

        public Task<UserDevice?> GetByIdAsync(Guid id)
            => _db.UserDevices.FirstOrDefaultAsync(d => d.Id == id);

        public async Task AddAsync(UserDevice device)
            => await _db.UserDevices.AddAsync(device);
    }
}
