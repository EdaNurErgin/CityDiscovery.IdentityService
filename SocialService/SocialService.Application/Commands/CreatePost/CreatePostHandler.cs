using MediatR;
using SocialService.Application.Interfaces;
using SocialService.Domain.Entities;
using SocialService.Shared.Common.Events.Social;

namespace SocialService.Application.Commands.CreatePost
{
    public class CreatePostHandler : IRequestHandler<CreatePostCommand, Guid>
    {
        private readonly IPostRepository _postRepository;
        private readonly IVenueServiceClient _venueServiceClient;
        private readonly IMediator _mediator;

        // Handler, veritabanı işlemleri için bir repository'e ihtiyaç duyar.
        // Bu repository'i "Dependency Injection" ile alacağız.
        public CreatePostHandler (IPostRepository postRepository, IVenueServiceClient venueServiceClient, IMediator mediator)
        {
            _postRepository = postRepository;
            _venueServiceClient = venueServiceClient;
            _mediator = mediator;
        }

        public async Task<Guid> Handle(CreatePostCommand request, CancellationToken cancellationToken)
        {
            // 1. Mekanın var olup olmadığını kontrol et.
            var venueExists = await _venueServiceClient.VenueExistsAsync(request.VenueId);
            if (!venueExists)
            {
                // Mekan yoksa bir hata fırlat. Bu hata API katmanında yakalanıp 400 Bad Request'e çevrilebilir.
                throw new Exception("Invalid VenueId"); // İleride burası için özel bir "NotFoundException" oluşturabiliriz.
            }

            // 2. Kontrol başarılıysa Post varlığını oluştur.
            var newPost = new Post
            {
                UserId = request.UserId,
                VenueId = request.VenueId, // Yeni alanı ekliyoruz
                Content = request.Content,
                CreatedDate = DateTime.UtcNow
            };

            // 3. Veritabanına kaydet.
            await _postRepository.AddAsync(newPost);

            //4. Olayı oluştur ve MediatR ile yayınla
            var postCreatedEvent = new PostCreatedEvent(
            postId: newPost.Id,
            userId: newPost.UserId,
            content: newPost.Content,
            createdDate: newPost.CreatedDate);

            await _mediator.Publish(postCreatedEvent, cancellationToken);


            return newPost.Id;

        }
    }
}
