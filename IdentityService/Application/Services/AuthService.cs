using IdentityService.Application.DTOs.Auth;
using IdentityService.Application.Interfaces;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Enums;
using System.Security.Cryptography;
using IdentityService.Infrastructure.Repositories;

// MassTransit ve Event sözleşmesi
using MassTransit; 
using IdentityService.Shared.MessageBus.Identity;

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

        //public async Task<AuthResultDto> RegisterAsync(RegisterRequest r)
        //{
        //    if (await _users.EmailExistsAsync(r.Email))
        //        throw new InvalidOperationException("Email already exists");

        //    if (await _users.UserNameExistsAsync(r.UserName))
        //        throw new InvalidOperationException("Username already exists");

        //    var user = new User
        //    {
        //        UserName = r.UserName,
        //        Email = r.Email,
        //        PasswordHash = _hasher.Hash(r.Password),
        //        Role = r.Role 
        //    };

        //    //await _users.AddAsync(user);
        //    //await _uow.SaveChangesAsync();

        //    //try
        //    //{
        //    //    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
        //    //    await _publish.Publish(new UserCreatedEvent(
        //    //        user.Id,
        //    //        user.UserName,
        //    //        user.Email,
        //    //        user.Role.ToString(),
        //    //        DateTime.UtcNow
        //    //    ), cts.Token);
        //    //}
        //    //catch {  }

        //    //var access = _jwt.CreateAccessToken(user);
        //    await _users.AddAsync(user);
        //    await _uow.SaveChangesAsync();

        //    // --- LOGLAMA EKLEYEREK GÜNCELLEDİĞİMİZ KISIM ---
        //    try
        //    {
        //        Console.WriteLine($"[IdentityService] Kullanıcı oluşturuldu ({user.UserName}). Event gönderiliyor...");

        //        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5)); // Süreyi biraz artırdım

        //        await _publish.Publish(new UserCreatedEvent(
        //            user.Id,
        //            user.UserName,
        //            user.Email,
        //            user.Role.ToString(), // <-- BURAYA DİKKAT! "Owner" string olarak gidiyor mu?
        //            DateTime.UtcNow
        //        ), cts.Token);

        //        Console.WriteLine("[IdentityService] UserCreatedEvent başarıyla RabbitMQ'ya gönderildi.");
        //    }
        //    catch (Exception ex)
        //    {
        //        // HATA YUTMA, LOGLA!
        //        Console.WriteLine($"[IdentityService HATA] Event gönderilemedi! Sebebi: {ex.Message}");
        //        if (ex.InnerException != null)
        //        {
        //            Console.WriteLine($"[IdentityService HATA DETAY] Inner: {ex.InnerException.Message}");
        //        }
        //    }
        //    // -----------------------------------------------

        //    var access = _jwt.CreateAccessToken(user);
        //    var refresh = await IssueRefreshTokenForNewDeviceAsync(user, Guid.NewGuid().ToString(), "init");
        //    return new AuthResultDto { AccessToken = access, RefreshToken = refresh.Token, UserId = user.Id };
        //}

        //public async Task<AuthResultDto> RegisterAsync(RegisterRequest r)
        //{
        //    // TÜM METODU TRY-CATCH İÇİNE ALIYORUZ
        //    try
        //    {
        //        // 1. Validasyonlar
        //        if (await _users.EmailExistsAsync(r.Email))
        //            throw new InvalidOperationException($"Email already exists: {r.Email}");

        //        if (await _users.UserNameExistsAsync(r.UserName))
        //            throw new InvalidOperationException($"Username already exists: {r.UserName}");

        //        var user = new User
        //        {
        //            UserName = r.UserName,
        //            Email = r.Email,
        //            PasswordHash = _hasher.Hash(r.Password),
        //            Role = r.Role
        //        };

        //        // 2. Veritabanı Kaydı (BURASI PATLIYOR OLABİLİR)
        //        await _users.AddAsync(user);
        //        await _uow.SaveChangesAsync();

        //        // 3. Event Gönderimi
        //        try
        //        {
        //            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        //            await _publish.Publish(new UserCreatedEvent(
        //                user.Id,
        //                user.UserName,
        //                user.Email,
        //                user.Role.ToString(),
        //                DateTime.UtcNow
        //            ), cts.Token);
        //        }
        //        catch (Exception ex)
        //        {
        //            // Event hatası akışı bozmasın diye burası ayrı catch'te
        //            Console.WriteLine($"Event gönderilemedi: {ex.Message}");
        //        }

        //        // 4. Token Üretimi
        //        var access = _jwt.CreateAccessToken(user);
        //        var refresh = await IssueRefreshTokenForNewDeviceAsync(user, Guid.NewGuid().ToString(), "init");

        //        return new AuthResultDto { AccessToken = access, RefreshToken = refresh.Token, UserId = user.Id };
        //    }
        //    catch (Exception ex)
        //    {
        //        // --- BREAKPOINT'İ BURAYA KOY ---
        //        // Mouse ile 'ex' üzerine gelip InnerException'a bak.
        //        var message = ex.Message;
        //        var inner = ex.InnerException?.Message;

        //        Console.WriteLine($"[CRITICAL ERROR] RegisterAsync Failed: {message}");
        //        if (inner != null) Console.WriteLine($"[INNER ERROR]: {inner}");

        //        throw; // Hatayı tekrar fırlat ki API 500 dönsün
        //    }
        //}
        public async Task<AuthResultDto?> RegisterAsync(RegisterRequest r)
        {
            // 1. Ön Kontroller (Validasyonlar)
            bool exists = await _users.EmailExistsAsync(r.Email) || await _users.UserNameExistsAsync(r.UserName);
            if (exists)
            {
                Console.WriteLine("[Register] Email veya Kullanıcı adı zaten mevcut.");
                return null; // Veya hata mesajı içeren bir DTO dönebilirsin
            }

            try
            {
                // 2. Kullanıcıyı Oluştur ve Kaydet
                var user = new User
                {
                    UserName = r.UserName,
                    Email = r.Email,
                    PasswordHash = _hasher.Hash(r.Password),
                    Role = r.Role
                };

                await _users.AddAsync(user);
                await _uow.SaveChangesAsync();

                // 3. Event Gönderimi (Arka planda sessizce çalışır, hata olsa da sistemi durdurmaz)
                _ = Task.Run(async () =>
                {
                    try
                    {
                        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                        await _publish.Publish(new UserCreatedEvent(
                            user.Id, user.UserName, user.Email, user.Role.ToString(), DateTime.UtcNow
                        ), cts.Token);
                    }
                    catch (Exception ex) { Console.WriteLine($"[Event Error] {ex.Message}"); }
                });

                // 4. Yanıtı Hazırla
                return new AuthResultDto
                {
                    AccessToken = _jwt.CreateAccessToken(user),
                    RefreshToken = (await IssueRefreshTokenForNewDeviceAsync(user, Guid.NewGuid().ToString(), "init")).Token,
                    UserId = user.Id
                };
            }
            catch (Exception ex)
            {
                // Program durmaz, sadece log basar ve null döner.
                Console.WriteLine($"[Register Critical Error] {ex.Message}");
                return null;
            }
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

            // 🆕 SocialService'e login bildirimi gönder
            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
                await _publish.Publish(new UserLoggedInEvent(
                    user.Id,
                    user.UserName,
                    user.Email,
                    user.Role.ToString(),
                    r.DeviceId,
                    DateTime.UtcNow
                ), cts.Token);
            }
            catch { } // Event gönderilemezse login başarısız olmasın

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
