using Elastic.Clients.Elasticsearch;
using IdentityService.Domain.Entities.Elasticsearch;
using IdentityService.Shared.MessageBus.Identity;
using MassTransit;

namespace IdentityService.Application.Consumers
{
    public class UserCreatedEventConsumer : IConsumer<UserCreatedEvent>
    {
        private readonly ElasticsearchClient _elasticClient;

        public UserCreatedEventConsumer(ElasticsearchClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        public async Task Consume(ConsumeContext<UserCreatedEvent> context)
        {
            var message = context.Message;

            var userDocument = new UserDocument
            {
                Id = message.UserId,
                UserName = message.UserName,
                Email = message.Email,
                Role = message.Role
            };

            // Elasticsearch'e "users" index'ine kaydediyoruz
            await _elasticClient.IndexAsync(userDocument, idx => idx.Index("users"));
        }
    }
}