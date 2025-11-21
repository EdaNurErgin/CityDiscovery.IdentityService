using IdentityService.Application.DTOs.Users;
using Shared.Common.DTOs.Identity;

namespace IdentityService.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> GetByIdAsync(Guid id);
        Task<UserDto> UpdateProfileAsync(Guid id, UpdateProfileRequest request);
        Task<List<UserDto>> GetAllAsync();             // Admin listeleme
        Task<bool> ExistsAsync(Guid id);               // Exists endpoint'i için
        
        // 🆕 YENİ METODLAR - Diğer servisler için
        Task<List<UserDto>> GetBulkByIdsAsync(List<Guid> userIds);
        Task<string> GetUserRoleAsync(Guid id);
        Task<List<UserDto>> GetActiveUsersAsync();
        Task<List<UserDto>> GetUsersByCityAsync(string city);
    }
}
