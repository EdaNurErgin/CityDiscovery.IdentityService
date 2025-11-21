using System;
using System.Threading.Tasks;
using IdentityService.Domain.Entities;

namespace IdentityService.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> EmailExistsAsync(string email);
        Task AddAsync(User user);
        Task<User?> GetActiveByEmailAsync(string email);
        Task<User?> GetByIdAsync(Guid id);
        Task<List<User>> GetAllAsync();
        
        // 🆕 YENİ METODLAR - Diğer servisler için
        Task<List<User>> GetBulkByIdsAsync(List<Guid> userIds);
        Task<List<User>> GetActiveUsersAsync();
        Task<List<User>> GetUsersByCityAsync(string city);
    }
}
