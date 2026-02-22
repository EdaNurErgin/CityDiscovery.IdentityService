namespace IdentityService.Domain.Entities.Elasticsearch
{
    public class UserDocument
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Role { get; set; } = default!;
        public string? City { get; set; }
    }
}
