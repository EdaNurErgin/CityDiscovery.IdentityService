//using IdentityService.Application.DTOs.Users;
//using IdentityService.Application.Interfaces;
//using IdentityService.Domain.Entities;
//using IdentityService.Shared.Common.DTOs.Identity;
//using IdentityService.Shared.MessageBus.Identity; 
//using MassTransit;

//public class UserService : IUserService
//{
//    private readonly IUserRepository _users;
//    private readonly IUnitOfWork _uow;
//    private readonly IPublishEndpoint _publishEndpoint; 
//    private readonly IUserRepository _userRepository;


//    public UserService(IUserRepository users, IUnitOfWork uow, IPublishEndpoint publishEndpoint, IUserRepository userRepository)
//    {
//        _users = users; _uow = uow; _publishEndpoint = publishEndpoint; _userRepository = userRepository;
//    }

//    public async Task<UserDto> GetByIdAsync(Guid id)
//    {
//        var user = await _users.GetByIdAsync(id) ?? throw new KeyNotFoundException("User not found");
//        if (!user.IsActive) throw new InvalidOperationException("Inactive user");
//        return ToDto(user);
//    }


//    public async Task<UserDto> UpdateProfileAsync(Guid id, UpdateProfileRequest r)
//    {
//        var user = await _users.GetByIdAsync(id) ?? throw new KeyNotFoundException("User not found");
//        if (!user.IsActive) throw new InvalidOperationException("Inactive user");

//        // 1. Değişiklik Kontrolü (Gereksiz yere event atmamak için)
//        bool isIdentityChanged = false;

//        // Kullanıcı adı değişmiş mi?
//        if (!string.IsNullOrEmpty(r.UserName) && r.UserName != user.UserName)
//        {
//            user.UserName = r.UserName;
//            isIdentityChanged = true;
//        }

//        // Avatar değişmiş mi?
//        if (r.AvatarUrl != null && r.AvatarUrl != user.AvatarUrl)
//        {
//            user.AvatarUrl = r.AvatarUrl;
//            isIdentityChanged = true;
//        }

//        // 2. Diğer alanları güncelle (Bunlar event gerektirmez)
//        user.Bio = r.Bio ?? user.Bio;
//        user.City = r.City ?? user.City;
//        user.DateOfBirth = r.DateOfBirth ?? user.DateOfBirth;
//        user.UpdatedAt = DateTime.UtcNow;

//        // 3. Veritabanına kaydet
//        await _uow.SaveChangesAsync();

//        // 4. EĞER kritik bilgiler değiştiyse, "UserUpdatedEvent" fırlat
//        if (isIdentityChanged)
//        {
//            await _publishEndpoint.Publish(new UserUpdatedEvent(
//                user.Id,
//                user.UserName,
//                user.AvatarUrl
//            ));
//        }

//        return ToDto(user);
//    }

//    public async Task<List<UserDto>> GetAllAsync()
//        => (await _users.GetAllAsync()).Select(ToDto).ToList();

//    public async Task<bool> ExistsAsync(Guid id)
//        => (await _users.GetByIdAsync(id)) is not null;

//    public async Task<List<UserDto>> GetBulkByIdsAsync(List<Guid> userIds)
//    {
//        var users = await _users.GetBulkByIdsAsync(userIds);
//        return users.Where(u => u.IsActive).Select(ToDto).ToList();
//    }

//    public async Task<string> GetUserRoleAsync(Guid id)
//    {
//        var user = await _users.GetByIdAsync(id);
//        return user?.IsActive == true ? user.Role.ToString() : "Inactive";
//    }

//    public async Task<List<UserDto>> GetActiveUsersAsync()
//    {
//        var users = await _users.GetActiveUsersAsync();
//        return users.Select(ToDto).ToList();
//    }

//    public async Task<List<UserDto>> GetUsersByCityAsync(string city)
//    {
//        var users = await _users.GetUsersByCityAsync(city);
//        return users.Where(u => u.IsActive).Select(ToDto).ToList();
//    }

//    private static UserDto ToDto(User u) => new()
//    {
//        Id = u.Id,
//        UserName = u.UserName,
//        Email = u.Email,
//        Role = u.Role.ToString(),
//        Bio = u.Bio,
//        City = u.City,
//        DateOfBirth = u.DateOfBirth,
//        AvatarUrl = u.AvatarUrl
//    };

//    // UserService sınıfının içine eklenecek metod:
//    //public async Task DeleteAsync(Guid id)
//    //{
//    //    var user = await _users.GetByIdAsync(id);
//    //    if (user == null) throw new KeyNotFoundException("User not found");

//    //    // 1. Kullanıcıyı sil (veya IsActive = false yap)
//    //    // Eğer Hard Delete yapıyorsan:
//    //    _users.Remove(user);
//    //    // Eğer Soft Delete yapıyorsan:
//    //    // user.IsActive = false; 

//    //    await _uow.SaveChangesAsync();

//    //    // 2. Diğer servislere "Bu kullanıcı silindi" diye haber ver
//    //    // (Social servisi postlarını silecek, Review servisi yorumlarını silecek vs.)
//    //    await _publishEndpoint.Publish(new UserDeletedEvent(
//    //        user.Id,
//    //        user.UserName,
//    //        user.Email,
//    //        user.Role.ToString(),
//    //        DateTime.UtcNow
//    //    ));
//    //}
//    // UserService.cs dosyasının içine en alta ekleyin:

//    public async Task<List<UserDto>> GetUsersByRoleAsync(string role)
//    {
//        // 1. Repository'den kullanıcıları çek
//        var users = await _userRepository.GetUsersByRoleAsync(role);

//        // 2. Entity -> DTO Çevirimi Yap (Hata veren satırlar kaldırıldı)
//        var userDtos = users.Select(u => new UserDto
//        {
//            Id = u.Id,
//            UserName = u.UserName,
//            Email = u.Email

//            // Eğer User tablonuzda "Name" ve "Surname" varsa şu şekilde birleştirebilirsiniz:
//            // FullName = u.Name + " " + u.Surname, 

//            // Eğer User tablonuzda "City" varsa yorumu kaldırın:
//            // City = u.City 

//        }).ToList();

//        return userDtos;
//    }
//    public async Task DeleteAsync(Guid id)
//    {
//        var user = await _users.GetByIdAsync(id);
//        if (user == null) throw new KeyNotFoundException("User not found");

//        // 1. Veritabanı işlemini yap
//        _users.Remove(user);
//        await _uow.SaveChangesAsync(); // <-- Kullanıcı burada silindi bile!

//        // 2. Event Fırlatmayı "Güvenli Blok" içine al
//        try
//        {
//            // Cancellation Token eklemek de iyi bir pratiktir
//            var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token; // 5 sn bekle

//            await _publishEndpoint.Publish(new UserDeletedEvent(
//                user.Id,
//                user.UserName,
//                user.Email,
//                user.Role.ToString(),
//                DateTime.UtcNow
//            ), cancellationToken);
//        }
//        catch (Exception ex)
//        {
//            // Buraya mutlaka bir Logger ekleyin (Console.WriteLine geçici olarak iş görür)
//            Console.WriteLine($"Kullanıcı silindi AMA Event gönderilemedi! Hata: {ex.Message}");

//            // ÖNEMLİ: Hatayı 'throw' ile fırlatmıyoruz. 
//            // Böylece API 204 NoContent dönmeye devam edebiliyor.
//        }
//    }
//}


using IdentityService.Application.DTOs.Users;
using IdentityService.Application.Interfaces;
using IdentityService.Domain.Entities;
using IdentityService.Shared.Common.DTOs.Identity;
using IdentityService.Shared.MessageBus.Identity;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityService.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository; // Tek repository değişkeni
        private readonly IUnitOfWork _uow;
        private readonly IPublishEndpoint _publishEndpoint;

        public UserService(IUserRepository userRepository, IUnitOfWork uow, IPublishEndpoint publishEndpoint)
        {
            _userRepository = userRepository;
            _uow = uow;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<UserDto> GetByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("User not found");
            if (!user.IsActive) throw new InvalidOperationException("Inactive user");
            return ToDto(user);
        }

        public async Task<UserDto> UpdateProfileAsync(Guid id, UpdateProfileRequest r)
        {
            var user = await _userRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("User not found");
            if (!user.IsActive) throw new InvalidOperationException("Inactive user");

            bool isIdentityChanged = false;

            if (!string.IsNullOrEmpty(r.UserName) && r.UserName != user.UserName)
            {
                user.UserName = r.UserName;
                isIdentityChanged = true;
            }

            if (r.AvatarUrl != null && r.AvatarUrl != user.AvatarUrl)
            {
                user.AvatarUrl = r.AvatarUrl;
                isIdentityChanged = true;
            }

            user.Bio = r.Bio ?? user.Bio;
            user.City = r.City ?? user.City;
            user.DateOfBirth = r.DateOfBirth ?? user.DateOfBirth;
            user.UpdatedAt = DateTime.UtcNow;

            await _uow.SaveChangesAsync();

            if (isIdentityChanged)
            {
                // RECORD YAPISINA UYGUN GÜNCELLEME:
                await _publishEndpoint.Publish(new UserUpdatedEvent(
                    user.Id,
                    user.UserName,
                    user.AvatarUrl
                ));
            }

            return ToDto(user);
        }

        public async Task<List<UserDto>> GetAllAsync()
            => (await _userRepository.GetAllAsync()).Select(ToDto).ToList();

        public async Task<bool> ExistsAsync(Guid id)
            => (await _userRepository.GetByIdAsync(id)) is not null;

        public async Task<List<UserDto>> GetBulkByIdsAsync(List<Guid> userIds)
        {
            var users = await _userRepository.GetBulkByIdsAsync(userIds);
            return users.Where(u => u.IsActive).Select(ToDto).ToList();
        }

        public async Task<string> GetUserRoleAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user?.IsActive == true ? user.Role.ToString() : "Inactive";
        }

        public async Task<List<UserDto>> GetActiveUsersAsync()
        {
            var users = await _userRepository.GetActiveUsersAsync();
            return users.Select(ToDto).ToList();
        }

        public async Task<List<UserDto>> GetUsersByCityAsync(string city)
        {
            var users = await _userRepository.GetUsersByCityAsync(city);
            return users.Where(u => u.IsActive).Select(ToDto).ToList();
        }

        public async Task<List<UserDto>> GetUsersByRoleAsync(string role)
        {
            var users = await _userRepository.GetUsersByRoleAsync(role);
            return users.Select(ToDto).ToList();
        }

        // --- ASIL DEĞİŞİKLİK BURADA (RECORD Event Yapısına Uygun) ---
        public async Task DeleteAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) throw new KeyNotFoundException("User not found");

            // 1. Veritabanından Sil
            _userRepository.Remove(user);
            await _uow.SaveChangesAsync();

            // 2. Event Yayınla (Record Constructor Kullanarak)
            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

                await _publishEndpoint.Publish(new UserDeletedEvent(
                    user.Id,
                    user.UserName,
                    user.Email,
                    user.Role.ToString(),
                    DateTime.UtcNow
                ), cts.Token);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Warning] User {id} deleted but event failed: {ex.Message}");
            }
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
}
