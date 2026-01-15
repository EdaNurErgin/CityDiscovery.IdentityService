using IdentityService.Application.DTOs.Users;
using IdentityService.Application.Interfaces;
using IdentityService.Domain.Entities;
using IdentityService.Shared.Common.DTOs.Identity;
using MassTransit;
using IdentityService.Shared.MessageBus.Identity; 

public class UserService : IUserService
{
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _uow;
    private readonly IPublishEndpoint _publishEndpoint; // EKLENDİ

    public UserService(IUserRepository users, IUnitOfWork uow, IPublishEndpoint publishEndpoint)
    {
        _users = users; _uow = uow; _publishEndpoint = publishEndpoint;
    }

    public async Task<UserDto> GetByIdAsync(Guid id)
    {
        var user = await _users.GetByIdAsync(id) ?? throw new KeyNotFoundException("User not found");
        if (!user.IsActive) throw new InvalidOperationException("Inactive user");
        return ToDto(user);
    }

    //public async Task<UserDto> UpdateProfileAsync(Guid id, UpdateProfileRequest r)
    //{
    //    var user = await _users.GetByIdAsync(id) ?? throw new KeyNotFoundException("User not found");
    //    if (!user.IsActive) throw new InvalidOperationException("Inactive user");

    //    user.UserName = r.UserName ?? user.UserName;
    //    user.Bio = r.Bio ?? user.Bio;
    //    user.City = r.City ?? user.City;
    //    user.AvatarUrl = r.AvatarUrl ?? user.AvatarUrl;
    //    user.DateOfBirth = r.DateOfBirth ?? user.DateOfBirth;
    //    user.UpdatedAt = DateTime.UtcNow;

    //    await _uow.SaveChangesAsync();
    //    return ToDto(user);
    //}
    public async Task<UserDto> UpdateProfileAsync(Guid id, UpdateProfileRequest r)
    {
        var user = await _users.GetByIdAsync(id) ?? throw new KeyNotFoundException("User not found");
        if (!user.IsActive) throw new InvalidOperationException("Inactive user");

        // 1. Değişiklik Kontrolü (Gereksiz yere event atmamak için)
        bool isIdentityChanged = false;

        // Kullanıcı adı değişmiş mi?
        if (!string.IsNullOrEmpty(r.UserName) && r.UserName != user.UserName)
        {
            user.UserName = r.UserName;
            isIdentityChanged = true;
        }

        // Avatar değişmiş mi?
        if (r.AvatarUrl != null && r.AvatarUrl != user.AvatarUrl)
        {
            user.AvatarUrl = r.AvatarUrl;
            isIdentityChanged = true;
        }

        // 2. Diğer alanları güncelle (Bunlar event gerektirmez)
        user.Bio = r.Bio ?? user.Bio;
        user.City = r.City ?? user.City;
        user.DateOfBirth = r.DateOfBirth ?? user.DateOfBirth;
        user.UpdatedAt = DateTime.UtcNow;

        // 3. Veritabanına kaydet
        await _uow.SaveChangesAsync();

        // 4. EĞER kritik bilgiler değiştiyse, "UserUpdatedEvent" fırlat
        if (isIdentityChanged)
        {
            await _publishEndpoint.Publish(new UserUpdatedEvent(
                user.Id,
                user.UserName,
                user.AvatarUrl
            ));
        }

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

    // UserService sınıfının içine eklenecek metod:
    public async Task DeleteAsync(Guid id)
    {
        var user = await _users.GetByIdAsync(id);
        if (user == null) throw new KeyNotFoundException("User not found");

        // 1. Kullanıcıyı sil (veya IsActive = false yap)
        // Eğer Hard Delete yapıyorsan:
        _users.Remove(user);
        // Eğer Soft Delete yapıyorsan:
        // user.IsActive = false; 

        await _uow.SaveChangesAsync();

        // 2. Diğer servislere "Bu kullanıcı silindi" diye haber ver
        // (Social servisi postlarını silecek, Review servisi yorumlarını silecek vs.)
        await _publishEndpoint.Publish(new UserDeletedEvent(
            user.Id,
            user.UserName,
            user.Email,
            user.Role.ToString(),
            DateTime.UtcNow
        ));
    }
}
