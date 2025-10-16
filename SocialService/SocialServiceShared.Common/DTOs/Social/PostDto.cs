using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialService.Shared.Common.DTOs.Social
{
    public class PostDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public int LikeCount { get; set; } // Entity'de olmayan ama hesaplayacağımız bir alan
        public List<CommentDto> Comments { get; set; } = new List<CommentDto>();
    }
}
