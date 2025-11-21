using IdentityService.Application.DTOs.Users;
using IdentityService.Application.Interfaces;
using IdentityService.Domain.Entities;
using Shared.Common.DTOs.Identity;

public class UserService : IUserService
{
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _uow;

    public UserService(IUserRepository users, IUnitOfWork uow)
    {
        _users = users; _uow = uow;
    }

    public async Task<UserDto> GetByIdAsync(Guid id)
    {
        var user = await _users.GetByIdAsync(id) ?? throw new KeyNotFoundException("User not found");
        if (!user.IsActive) throw new InvalidOperationException("Inactive user");
        return ToDto(user);
    }

    public async Task<UserDto> UpdateProfileAsync(Guid id, UpdateProfileRequest r)
    {
        var user = await _users.GetByIdAsync(id) ?? throw new KeyNotFoundException("User not found");
        if (!user.IsActive) throw new InvalidOperationException("Inactive user");

        user.UserName = r.UserName ?? user.UserName;
        user.Bio = r.Bio ?? user.Bio;
        user.City = r.City ?? user.City;
        user.AvatarUrl = r.AvatarUrl ?? user.AvatarUrl;
        user.DateOfBirth = r.DateOfBirth ?? user.DateOfBirth;
        user.UpdatedAt = DateTime.UtcNow;

        await _uow.SaveChangesAsync();
        return ToDto(user);
    }

    public async Task<List<UserDto>> GetAllAsync()
        => (await _users.GetAllAsync()).Select(ToDto).ToList();

    public async Task<bool> ExistsAsync(Guid id)
        => (await _users.GetByIdAsync(id)) is not null;

    // 🆕 YENİ METODLAR - Diğer servisler için
    public async Task<List<UserDto>> GetBulkByIdsAsync(List<Guid> userIds)
    {
        var users = await _users.GetBulkByIdsAsync(userIds);
        return users.Where(u => u.IsActive).Select(ToDto).ToList();
    }

    public async Task<string> GetUserRoleAsync(Guid id)
    {
        var user = await _users.GetByIdAsync(id);
        return user?.IsActive == true ? user.Role.ToString() : "Inactive";
    }

    public async Task<List<UserDto>> GetActiveUsersAsync()
    {
        var users = await _users.GetActiveUsersAsync();
        return users.Select(ToDto).ToList();
    }

    public async Task<List<UserDto>> GetUsersByCityAsync(string city)
    {
        var users = await _users.GetUsersByCityAsync(city);
        return users.Where(u => u.IsActive).Select(ToDto).ToList();
    }

    private static UserDto ToDto(User u) => new()
    {
        Id = u.Id,
        UserName = u.UserName,
        Email = u.Email,
        Role = u.Role.ToString(),
        Bio = u.Bio,
        City = u.City,
        DateOfBirth = u.DateOfBirth,
        AvatarUrl = u.AvatarUrl
    };
}
