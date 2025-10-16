using MediatR;
using SocialService.Shared.Common.DTOs.Social;

namespace SocialService.Application.Queries.GetPost
{
    public class GetPostQuery : IRequest<PostDto>
    {
        public Guid Id { get; set; }
    }
}
