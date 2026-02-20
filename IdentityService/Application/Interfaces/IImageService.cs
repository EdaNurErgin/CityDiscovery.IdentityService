namespace IdentityService.Application.Interfaces
{
    public interface IImageService
    {
        Task<string> SaveAvatarAsync(IFormFile file);
    }
}
