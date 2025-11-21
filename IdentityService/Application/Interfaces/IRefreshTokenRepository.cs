using IdentityService.Domain.Entities;

namespace IdentityService.Application.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenWithIncludesAsync(string token);
        Task<RefreshToken?> GetByUserDeviceIdAsync(Guid userDeviceId);
        Task AddAsync(RefreshToken token);
        void Remove(RefreshToken token); 
    }
}
