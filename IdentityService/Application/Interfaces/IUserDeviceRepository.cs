using IdentityService.Domain.Entities;

namespace IdentityService.Application.Interfaces
{
    public interface IUserDeviceRepository
    {
        Task<UserDevice?> GetByUserAndDeviceAsync(Guid userId, string deviceId);
        Task<UserDevice?> GetByIdAsync(Guid id);
        Task AddAsync(UserDevice device);
    }
}
