using IdentityService.Domain.Entities;

namespace IdentityService.Application.Interfaces
{
    public interface IJwtTokenService
    {
        string CreateAccessToken(User user);
    }

}
