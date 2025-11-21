using IdentityService.Application.DTOs.Auth;
using IdentityService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        public AuthController(IAuthService auth) { _auth = auth; }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResultDto>> Register(RegisterRequest request)
            => Ok(await _auth.RegisterAsync(request));

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResultDto>> Login(LoginRequest request)
            => Ok(await _auth.LoginAsync(request));

        public record RefreshRequest(string RefreshToken, string DeviceId);


        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResultDto>> Refresh(RefreshRequest r)
        {
            try { return Ok(await _auth.RefreshAsync(r.RefreshToken, r.DeviceId)); }
            catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
        }

    }

}
