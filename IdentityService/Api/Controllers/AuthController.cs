using IdentityService.Application.DTOs.Auth;
using IdentityService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Api.Controllers
{
    /// <summary>
    /// Authentication ve Authorization işlemleri için endpoint'ler
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        public AuthController(IAuthService auth) { _auth = auth; }

        /// <summary>
        /// Yeni kullanıcı kaydı oluşturur
        /// </summary>
        /// <param name="request">Kayıt bilgileri (UserName, Email, Password, Role)</param>
        /// <returns>Access token, Refresh token ve UserId içeren auth sonucu</returns>
        /// <response code="200">Kayıt başarılı, token'lar döner</response>
        /// <response code="400">Email veya Username zaten kullanılıyor</response>
        /// <remarks>
        /// Örnek istek:
        /// 
        ///     POST /api/Auth/register
        ///     {
        ///         "userName": "johndoe",
        ///         "email": "john@example.com",
        ///         "password": "SecurePass123!",
        ///         "role": 0
        ///     }
        /// 
        /// Role değerleri: 0=User, 1=Admin, 2=Owner
        /// </remarks>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthResultDto>> Register(RegisterRequest request)
            => Ok(await _auth.RegisterAsync(request));

        /// <summary>
        /// Kullanıcı girişi yapar ve token'ları döner
        /// </summary>
        /// <param name="request">Login bilgileri (Email, Password, DeviceId, DeviceName)</param>
        /// <returns>Access token, Refresh token ve UserId içeren auth sonucu</returns>
        /// <response code="200">Giriş başarılı, token'lar döner</response>
        /// <response code="400">Geçersiz email veya şifre</response>
        /// <remarks>
        /// Örnek istek:
        /// 
        ///     POST /api/Auth/login
        ///     {
        ///         "email": "john@example.com",
        ///         "password": "SecurePass123!",
        ///         "deviceId": "device-123",
        ///         "deviceName": "iPhone 15"
        ///     }
        /// </remarks>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthResultDto>> Login(LoginRequest request)
            => Ok(await _auth.LoginAsync(request));

        /// <summary>
        /// Refresh token ile yeni access token alır
        /// </summary>
        /// <param name="r">Refresh token ve DeviceId</param>
        /// <returns>Yeni access token, refresh token ve UserId</returns>
        /// <response code="200">Token yenileme başarılı</response>
        /// <response code="400">Geçersiz veya süresi dolmuş refresh token</response>
        /// <remarks>
        /// Örnek istek:
        /// 
        ///     POST /api/Auth/refresh-token
        ///     {
        ///         "refreshToken": "base64-encoded-token",
        ///         "deviceId": "device-123"
        ///     }
        /// </remarks>
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthResultDto>> Refresh(RefreshRequest r)
        {
            try { return Ok(await _auth.RefreshAsync(r.RefreshToken, r.DeviceId)); }
            catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
        }

        /// <summary>
        /// Refresh token isteği için DTO
        /// </summary>
        public record RefreshRequest(string RefreshToken, string DeviceId);
    }
}
