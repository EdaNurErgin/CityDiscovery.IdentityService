using SocialService.Domain.Common;


namespace SocialService.Domain.Entities
{
    public class PostPhoto: AuditableEntity
    {
        public Guid PostId { get; set; }
        public string ImageUrl { get; set; }
        public Post Post { get; set; }
    }
}
