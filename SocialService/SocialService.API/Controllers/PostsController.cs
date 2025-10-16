using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialService.Application.Commands.CreatePost;
using SocialService.Application.Queries.GetPost;

namespace SocialService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PostsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostCommand command)
        {
            // Gelen isteği (command) MediatR'a gönderiyoruz.
            // MediatR doğru handler'ı (CreatePostHandler) bulup çalıştıracak.
            var postId = await _mediator.Send(command);

            // Başarılı bir şekilde oluşturulduğunda, yeni post'un ID'si ile birlikte 201 Created yanıtı dönüyoruz.
            return CreatedAtAction(nameof(CreatePost), new { id = postId }, null);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPostById(Guid id)
        {
            var query = new GetPostQuery { Id = id };
            var post = await _mediator.Send(query);

            if (post == null)
            {
                return NotFound();
            }
            return Ok(post);
        }
    }
}
