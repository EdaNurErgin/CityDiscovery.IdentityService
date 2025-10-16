using SocialService.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialService.Domain.Entities
{
    public class PostComment: AuditableEntity
    {
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
        public Post Post { get; set; }
        public string Content { get; set; }
    }
}
