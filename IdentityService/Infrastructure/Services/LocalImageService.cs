using IdentityService.Application.Interfaces;

namespace IdentityService.Infrastructure.Services
{
    public class LocalImageService : IImageService
    {
        private readonly IWebHostEnvironment _environment;

        public LocalImageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> SaveAvatarAsync(IFormFile file)
        {
            if (file == null || file.Length == 0) return null;

            string webRootPath = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
            var uploadsFolder = Path.Combine(webRootPath, "uploads", "avatars");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileExtension = Path.GetExtension(file.FileName);
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // Diğer servislerin erişebilmesi için tam URL veya erişilebilir path dönülmeli
            return $"/uploads/avatars/{uniqueFileName}";
        }
    }
}
