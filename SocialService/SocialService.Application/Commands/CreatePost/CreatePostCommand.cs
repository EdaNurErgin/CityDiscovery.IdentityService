using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialService.Application.Commands.CreatePost
{
    public class CreatePostCommand : IRequest<Guid>
    {
        public Guid UserId { get; set; }
        public string Content { get; set; }
        public Guid VenueId { get; set; } 
        public List<string> PhotoUrls { get; set; }
    }
}
