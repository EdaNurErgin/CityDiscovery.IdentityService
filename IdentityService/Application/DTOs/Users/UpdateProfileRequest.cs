namespace IdentityService.Application.DTOs.Users
{
    public class UpdateProfileRequest
    {
        public string? UserName { get; set; }
        public string? Bio { get; set; }
        public string? City { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? AvatarUrl { get; set; }
    }
}
