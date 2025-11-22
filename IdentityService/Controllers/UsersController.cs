using IdentityService.Application.DTOs.Users;
using IdentityService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IdentityService.Shared.Common.DTOs.Identity;

namespace IdentityService.Controllers
{
    /// <summary>
    /// Kullanıcı yönetimi işlemleri için endpoint'ler
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _users;
        public UsersController(IUserService users) => _users = users;

        /// <summary>
        /// Belirli bir kullanıcının detaylarını getirir
        /// </summary>
        /// <param name="id">Kullanıcı ID (GUID)</param>
        /// <returns>Kullanıcı bilgileri</returns>
        /// <response code="200">Kullanıcı bulundu</response>
        /// <response code="401">Yetkilendirme hatası - Token gerekli</response>
        /// <response code="404">Kullanıcı bulunamadı</response>
        [Authorize]
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDto>> GetUser(Guid id)
            => Ok(await _users.GetByIdAsync(id));

        /// <summary>
        /// Kullanıcı profil bilgilerini günceller
        /// </summary>
        /// <param name="id">Kullanıcı ID (GUID)</param>
        /// <param name="request">Güncellenecek profil bilgileri</param>
        /// <returns>Güncellenmiş kullanıcı bilgileri</returns>
        /// <response code="200">Profil başarıyla güncellendi</response>
        /// <response code="401">Yetkilendirme hatası - Token gerekli</response>
        /// <response code="404">Kullanıcı bulunamadı</response>
        [Authorize]
        [HttpPut("{id:guid}/profile")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDto>> UpdateProfile(Guid id, UpdateProfileRequest request)
            => Ok(await _users.UpdateProfileAsync(id, request));

        /// <summary>
        /// Tüm kullanıcıları listeler (Sadece Admin)
        /// </summary>
        /// <returns>Kullanıcı listesi</returns>
        /// <response code="200">Kullanıcı listesi döner</response>
        /// <response code="401">Yetkilendirme hatası - Token gerekli</response>
        /// <response code="403">Yetki hatası - Admin rolü gerekli</response>
        /// <remarks>
        /// Bu endpoint sadece Admin rolüne sahip kullanıcılar tarafından kullanılabilir.
        /// 
        /// Örnek istek:
        /// 
        ///     GET /api/Users/all
        ///     Authorization: Bearer {admin_token}
        /// </remarks>
        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        [ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<List<UserDto>>> GetAll()
            => Ok(await _users.GetAllAsync());

        /// <summary>
        /// Kullanıcının var olup olmadığını kontrol eder
        /// </summary>
        /// <param name="id">Kullanıcı ID (GUID)</param>
        /// <returns>Kullanıcı varsa true, yoksa false</returns>
        /// <response code="200">Kontrol sonucu döner</response>
        /// <remarks>
        /// Bu endpoint public'tir ve token gerektirmez. Diğer mikroservisler tarafından kullanılabilir.
        /// 
        /// Örnek istek:
        /// 
        ///     GET /api/Users/{guid}/exists
        /// </remarks>
        [AllowAnonymous]
        [HttpGet("{id:guid}/exists")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> Exists(Guid id)
            => Ok(await _users.ExistsAsync(id));

        /// <summary>
        /// Birden fazla kullanıcının bilgilerini toplu olarak getirir
        /// </summary>
        /// <param name="userIds">Kullanıcı ID listesi (GUID array)</param>
        /// <returns>Kullanıcı listesi</returns>
        /// <response code="200">Kullanıcı listesi döner</response>
        /// <remarks>
        /// Diğer mikroservisler için kullanılır. Örneğin: Post feed'de 20 post'un sahibi kullanıcıları getirmek için.
        /// 
        /// Örnek istek:
        /// 
        ///     POST /api/Users/bulk
        ///     [
        ///         "guid-1",
        ///         "guid-2",
        ///         "guid-3"
        ///     ]
        /// </remarks>
        [HttpPost("bulk")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<UserDto>>> GetBulkUsers([FromBody] List<Guid> userIds)
            => Ok(await _users.GetBulkByIdsAsync(userIds));

        /// <summary>
        /// Kullanıcının rolünü getirir
        /// </summary>
        /// <param name="id">Kullanıcı ID (GUID)</param>
        /// <returns>Kullanıcı rolü (User, Admin, Owner) veya "Inactive"</returns>
        /// <response code="200">Kullanıcı rolü döner</response>
        /// <remarks>
        /// Diğer mikroservisler için kullanılır. Admin yetkisi kontrolü için kullanılabilir.
        /// </remarks>
        [HttpGet("{id:guid}/role")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<ActionResult<string>> GetUserRole(Guid id)
            => Ok(await _users.GetUserRoleAsync(id));

        /// <summary>
        /// Aktif kullanıcıları listeler
        /// </summary>
        /// <returns>Aktif kullanıcı listesi</returns>
        /// <response code="200">Aktif kullanıcı listesi döner</response>
        /// <remarks>
        /// Sadece IsActive=true olan kullanıcıları döner. Diğer mikroservisler için kullanılır.
        /// </remarks>
        [HttpGet("active")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<UserDto>>> GetActiveUsers()
            => Ok(await _users.GetActiveUsersAsync());

        /// <summary>
        /// Belirli bir şehirdeki kullanıcıları listeler
        /// </summary>
        /// <param name="city">Şehir adı</param>
        /// <returns>Şehirdeki kullanıcı listesi</returns>
        /// <response code="200">Kullanıcı listesi döner</response>
        /// <remarks>
        /// Örnek istek:
        /// 
        ///     GET /api/Users/by-city/Istanbul
        /// </remarks>
        [HttpGet("by-city/{city}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<UserDto>>> GetUsersByCity(string city)
            => Ok(await _users.GetUsersByCityAsync(city));
    }
}