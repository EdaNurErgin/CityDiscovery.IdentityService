using SocialService.Domain.Common;

namespace SocialService.Domain.Entities
{
    public class PostLike: AuditableEntity
    {
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
        public Post Post { get; set; }
        public DateTime LikedDate { get; set; }
    }
}
