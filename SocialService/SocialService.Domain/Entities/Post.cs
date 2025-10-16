using SocialService.Domain.Common;


namespace SocialService.Domain.Entities
{
    public class Post : BaseEntity
    {
        public Guid UserId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid VenueId { get; set; }
        public ICollection<PostComment> Comments { get; set; } = new List<PostComment>();
        public ICollection<PostLike> Likes { get; set; } = new List<PostLike>();
        public ICollection<PostPhoto> Photos { get; set; } = new List<PostPhoto>();

    }
}
