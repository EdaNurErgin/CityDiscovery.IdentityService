using IdentityService.Application.DTOs.Users;
using IdentityService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Common.DTOs.Identity;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _users;
    public UsersController(IUserService users) => _users = users;

    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserDto>> GetUser(Guid id)
        => Ok(await _users.GetByIdAsync(id));

    [Authorize]
    [HttpPut("{id:guid}/profile")]
    public async Task<ActionResult<UserDto>> UpdateProfile(Guid id, UpdateProfileRequest request)
        => Ok(await _users.UpdateProfileAsync(id, request));

    [Authorize(Roles = "Admin")]
    [HttpGet("all")]
    public async Task<ActionResult<List<UserDto>>> GetAll()
        => Ok(await _users.GetAllAsync());

    [AllowAnonymous]
    [HttpGet("{id:guid}/exists")]
    public async Task<ActionResult<bool>> Exists(Guid id)
        => Ok(await _users.ExistsAsync(id));

    // 🆕 YENİ ENDPOINT'LER - Diğer servisler için
    
    /// <summary>
    /// Diğer mikroservisler için kullanıcı bilgilerini toplu olarak getirir
    /// </summary>
    [HttpPost("bulk")]
    [AllowAnonymous] // Internal service call için
    public async Task<ActionResult<List<UserDto>>> GetBulkUsers([FromBody] List<Guid> userIds)
        => Ok(await _users.GetBulkByIdsAsync(userIds));

    /// <summary>
    /// Kullanıcı rolünü kontrol eder (Admin yetkisi için)
    /// </summary>
    [HttpGet("{id:guid}/role")]
    [AllowAnonymous] // Internal service call için
    public async Task<ActionResult<string>> GetUserRole(Guid id)
        => Ok(await _users.GetUserRoleAsync(id));

    /// <summary>
    /// Aktif kullanıcıları getirir (sadece aktif olanlar)
    /// </summary>
    [HttpGet("active")]
    [AllowAnonymous] // Internal service call için
    public async Task<ActionResult<List<UserDto>>> GetActiveUsers()
        => Ok(await _users.GetActiveUsersAsync());

    /// <summary>
    /// Belirli şehirdeki kullanıcıları getirir
    /// </summary>
    [HttpGet("by-city/{city}")]
    [AllowAnonymous] // Internal service call için
    public async Task<ActionResult<List<UserDto>>> GetUsersByCity(string city)
        => Ok(await _users.GetUsersByCityAsync(city));
}