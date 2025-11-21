namespace IdentityService.Shared.Common.DTOs.Identity
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Role { get; set; } = default!;

        public string? Bio { get; set; }
        public string? City { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
}

