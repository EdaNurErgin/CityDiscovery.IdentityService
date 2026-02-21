using IdentityService.Application.DTOs.Users;
using IdentityService.Application.Interfaces;
using IdentityService.Domain.Entities;
using IdentityService.Shared.Common.DTOs.Identity;
using IdentityService.Shared.MessageBus.Identity;
using MassTransit;


namespace IdentityService.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository; // Tek repository değişkeni
        private readonly IUnitOfWork _uow;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IImageService _imageService; // Avatar yükleme için eklenen servis

        public UserService(IUserRepository userRepository, IUnitOfWork uow, IPublishEndpoint publishEndpoint, IImageService imageService)
        {
            _userRepository = userRepository;
            _uow = uow;
            _publishEndpoint = publishEndpoint;
            _imageService = imageService;
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

            //if (r.AvatarUrl != null && r.AvatarUrl != user.AvatarUrl)
            //{
            //    user.AvatarUrl = r.AvatarUrl;
            //    isIdentityChanged = true;
            //}

            if (!string.IsNullOrEmpty(r.AvatarUrl) && r.AvatarUrl != user.AvatarUrl)
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

        public async Task<UserDto> UploadAvatarAsync(Guid id, IFormFile file)
        {
            var user = await _userRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("User not found");

            // 1. Resmi kaydet
            var avatarUrl = await _imageService.SaveAvatarAsync(file);

            // 2. Kullanıcıyı güncelle
            user.AvatarUrl = avatarUrl;
            user.UpdatedAt = DateTime.UtcNow;

            await _uow.SaveChangesAsync();

            // 3. Diğer servislere (Social, Review vb.) avatarın değiştiğini bildir
            await _publishEndpoint.Publish(new UserUpdatedEvent(
                user.Id,
                user.UserName,
                user.AvatarUrl
            ));

            return ToDto(user);
        }
    }
}
