using IdentityService.Application.Interfaces;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Enums;
using IdentityService.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace IdentityService.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IdentityDbContext _db;

        public UserRepository(IdentityDbContext db) => _db = db;

        public Task<bool> EmailExistsAsync(string email)
            => _db.Users.AnyAsync(u => u.Email == email);

        public Task<User?> GetActiveByEmailAsync(string email)
            => _db.Users.FirstOrDefaultAsync(u => u.Email == email && u.IsActive);

        public Task<bool> UserNameExistsAsync(string userName)
            => _db.Users.AnyAsync(u => u.UserName == userName);

        public Task<User?> GetByIdAsync(Guid id)
            => _db.Users.FirstOrDefaultAsync(u => u.Id == id);

        public async Task AddAsync(User user)
            => await _db.Users.AddAsync(user);

        public Task<List<User>> GetAllAsync()
            => _db.Users.AsNoTracking().ToListAsync();

        // 🆕 YENİ METODLAR - Diğer servisler için
        public async Task<List<User>> GetBulkByIdsAsync(List<Guid> userIds)
            => await _db.Users
                .Where(u => userIds.Contains(u.Id))
                .AsNoTracking()
                .ToListAsync();

        public async Task<List<User>> GetActiveUsersAsync()
            => await _db.Users
                .Where(u => u.IsActive)
                .AsNoTracking()
                .ToListAsync();

        public async Task<List<User>> GetUsersByCityAsync(string city)
            => await _db.Users
                .Where(u => u.City == city)
                .AsNoTracking()
                .ToListAsync();


        public void Remove(User user)
        {
            _db.Users.Remove(user);
        }


        

        public async Task<List<User>> GetUsersByRoleAsync(string roleName)
        {
           
            if (!Enum.TryParse(typeof(UserRole), roleName, true, out var parsedRole))
            {
                // Eğer geçersiz bir rol ismi geldiyse boş liste dönelim.
                return new List<User>();
            }

            
            int roleId = (int)parsedRole;

         
            var sql = "SELECT * FROM Users WHERE Role = {0}";

            return await _db.Users
                .FromSqlRaw(sql, roleId)
                .AsNoTracking()
                .ToListAsync();
        }

    }
}
