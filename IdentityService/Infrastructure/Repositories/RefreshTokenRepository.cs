using IdentityService.Application.Interfaces;
using IdentityService.Domain.Entities;
using IdentityService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Infrastructure.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly IdentityDbContext _db;
        public RefreshTokenRepository(IdentityDbContext db) => _db = db;

        public Task<RefreshToken?> GetByTokenWithIncludesAsync(string token)
            => _db.RefreshTokens
                  .Include(t => t.User)
                  .Include(t => t.UserDevice)
                  .FirstOrDefaultAsync(t => t.Token == token);

        public Task<RefreshToken?> GetByUserDeviceIdAsync(Guid userDeviceId)
            => _db.RefreshTokens.FirstOrDefaultAsync(t => t.UserDeviceId == userDeviceId);

        public async Task AddAsync(RefreshToken token)
            => await _db.RefreshTokens.AddAsync(token);

        public void Remove(RefreshToken token)
           => _db.RefreshTokens.Remove(token);

    }
}
