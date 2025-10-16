using MediatR;
using SocialService.Shared.Common.DTOs.Social;
using SocialService.Application.Interfaces;

namespace SocialService.Application.Queries.GetPost
{
    public class GetPostHandler : IRequestHandler<GetPostQuery, PostDto>
    {
        private readonly IPostRepository _postRepository;
        public GetPostHandler(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }
        public async Task<PostDto> Handle(GetPostQuery request, CancellationToken cancellationToken)
        {
            var post = await _postRepository.GetByIdAsync(request.Id);

            if (post == null)
            {
                // Gönderi bulunamazsa null dönebiliriz veya bir exception fırlatabiliriz.
                // Şimdilik null dönelim. Controller bunu 404 Not Found olarak yorumlayacak.
                return null;
            }

            // Post varlığını PostDto'ya dönüştürüyoruz (map'leme).
            var postDto = new PostDto
            {
                Id = post.Id,
                UserId = post.UserId,
                Content = post.Content,
                CreatedDate = post.CreatedDate,
                LikeCount = post.Likes.Count, // Beğeni sayısını hesaplıyoruz
                Comments = post.Comments.Select(c => new CommentDto
                {
                    Id = c.Id,
                    UserId = c.UserId,
                    Content = c.Content,
                    CreatedDate = c.CreatedDate
                }).ToList()
            };

            return postDto;
        }
    }
}
