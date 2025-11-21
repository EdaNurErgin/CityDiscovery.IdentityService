using IdentityService.Application.DTOs.Auth;
using IdentityService.Application.Interfaces;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Enums;
using System.Security.Cryptography;
using IdentityService.Infrastructure.Repositories;

// MassTransit ve Event sözleşmesi
using MassTransit; 
using Shared.MessageBus.Identity; // Shared/MessageBus/Identity/UserCreatedEvent.cs

namespace IdentityService.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _users;
        private readonly IUserDeviceRepository _devices;
        private readonly IRefreshTokenRepository _tokens;
        private readonly IUnitOfWork _uow;

        private readonly IPasswordHasher _hasher;
        private readonly IJwtTokenService _jwt;
        private readonly IPublishEndpoint _publish;

        public AuthService(
            IUserRepository users,
            IUserDeviceRepository devices,
            IRefreshTokenRepository tokens,
            IUnitOfWork uow,
            IPasswordHasher hasher,
            IJwtTokenService jwt,
            IPublishEndpoint publish)
        {
            _users = users;
            _devices = devices;
            _tokens = tokens;
            _uow = uow;
            _hasher = hasher;
            _jwt = jwt;
            _publish = publish;
        }

        public async Task<AuthResultDto> RegisterAsync(RegisterRequest r)
        {
            if (await _users.EmailExistsAsync(r.Email))
                throw new InvalidOperationException("Email already exists");

            var user = new User
            {
                UserName = r.UserName,
                Email = r.Email,
                PasswordHash = _hasher.Hash(r.Password),
                Role = r.Role 
            };

            await _users.AddAsync(user);
            await _uow.SaveChangesAsync();

            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
                await _publish.Publish(new UserCreatedEvent(
                    user.Id,
                    user.UserName,
                    user.Email,
                    user.Role.ToString(),
                    DateTime.UtcNow
                ), cts.Token);
            }
            catch {  }

            var access = _jwt.CreateAccessToken(user);
            var refresh = await IssueRefreshTokenForNewDeviceAsync(user, Guid.NewGuid().ToString(), "init");
            return new AuthResultDto { AccessToken = access, RefreshToken = refresh.Token, UserId = user.Id };
        }

    

        public async Task<AuthResultDto> LoginAsync(LoginRequest r)
        {
            var user = await _users.GetActiveByEmailAsync(r.Email);
            if (user is null || !_hasher.Verify(user.PasswordHash, r.Password))
                throw new InvalidOperationException("Invalid credentials");

            var device = await _devices.GetByUserAndDeviceAsync(user.Id, r.DeviceId);
            if (device is null)
            {
                device = new UserDevice { UserId = user.Id, DeviceId = r.DeviceId, DeviceName = r.DeviceName };
                await _devices.AddAsync(device);
                await _uow.SaveChangesAsync();
            }

            // ❗ 1) Aynı cihaz için varsa eski token'ı sil ve yazdır (Unique index çatışmasından kaçınmak için)
            var old = await _tokens.GetByUserDeviceIdAsync(device.Id);
            if (old != null)
            {
                _tokens.Remove(old);
                await _uow.SaveChangesAsync(); // önce silme committed olsun
            }

            // ❗ 2) Yeni refresh token üret ve yaz
            var refresh = await IssueRefreshTokenAsync(user, device.Id);

            // ❗ 3) Access token üret ve hepsini kaydet
            var access = _jwt.CreateAccessToken(user);
            await _uow.SaveChangesAsync();

            return new AuthResultDto { AccessToken = access, RefreshToken = refresh.Token, UserId = user.Id };
        }



      
        public async Task<AuthResultDto> RefreshAsync(string refreshToken, string deviceId)
        {
            var token = await _tokens.GetByTokenWithIncludesAsync(refreshToken);
            if (token is null || token.IsRevoked || token.ExpiresAt <= DateTime.UtcNow)
                throw new InvalidOperationException("Invalid refresh token");

            if (token.UserDevice.DeviceId != deviceId)
                throw new InvalidOperationException("Device mismatch");

            // ❗ Eski token'ı sil (revoke yerine)
            _tokens.Remove(token);
            await _uow.SaveChangesAsync(); // önce silme yazılsın

            // ❗ Aynı cihaz için yeni refresh token oluştur
            var newRt = await IssueRefreshTokenAsync(token.User, token.UserDeviceId);

            var newAt = _jwt.CreateAccessToken(token.User);
            await _uow.SaveChangesAsync();

            return new AuthResultDto { AccessToken = newAt, RefreshToken = newRt.Token, UserId = token.User.Id };
        }





        // --- Private helpers ---
        private async Task<RefreshToken> IssueRefreshTokenAsync(User user, Guid userDeviceId)
        {
            var rt = new RefreshToken
            {
                UserId = user.Id,
                UserDeviceId = userDeviceId,
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                ExpiresAt = DateTime.UtcNow.AddDays(30)
            };
            await _tokens.AddAsync(rt);
            return rt;
        }

        private async Task<RefreshToken> IssueRefreshTokenForNewDeviceAsync(User user, string deviceId, string deviceName)
        {
            var device = new UserDevice { UserId = user.Id, DeviceId = deviceId, DeviceName = deviceName };
            await _devices.AddAsync(device);
            await _uow.SaveChangesAsync();
            return await IssueRefreshTokenAsync(user, device.Id);
        }
    }
}
