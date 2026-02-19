using IdentityService.Domain.Entities;
using IdentityService.Shared.Common.DTOs.Identity;
using System;
using System.Threading.Tasks;

namespace IdentityService.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> EmailExistsAsync(string email);
        Task<bool> UserNameExistsAsync(string userName);
        Task AddAsync(User user);
        Task<User?> GetActiveByEmailAsync(string email);
        Task<User?> GetByIdAsync(Guid id);
        Task<List<User>> GetAllAsync();
        Task<List<User>> GetBulkByIdsAsync(List<Guid> userIds);
        Task<List<User>> GetActiveUsersAsync();
        Task<List<User>> GetUsersByCityAsync(string city);
        void Remove(User user);
        Task<List<User>> GetUsersByRoleAsync(string role);
    }
}
