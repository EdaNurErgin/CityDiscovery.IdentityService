using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialService.Shared.Common.Events.Social
{
    public class PostCreatedEvent : INotification
    {
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }

        public PostCreatedEvent(Guid postId, Guid userId, string content, DateTime createdDate)
        {
            PostId = postId;
            UserId = userId;
            Content = content;
            CreatedDate = createdDate;
        }
    }
}
