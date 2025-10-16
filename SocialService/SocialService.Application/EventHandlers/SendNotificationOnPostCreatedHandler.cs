using MediatR;
using Microsoft.Extensions.Logging;
using SocialService.Shared.Common.Events.Social;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialService.Application.EventHandlers
{
    public class SendNotificationOnPostCreatedHandler :INotificationHandler<PostCreatedEvent>
    {
        private readonly ILogger<SendNotificationOnPostCreatedHandler> _logger;

        // Constructor üzerinden ILogger'ı enjekte ediyoruz.
        // .NET bunu bizim için otomatik olarak yapacak.
        public SendNotificationOnPostCreatedHandler(ILogger<SendNotificationOnPostCreatedHandler> logger)
        {
            _logger = logger;
        }
        public Task Handle(PostCreatedEvent notification, CancellationToken cancellationToken)
        {

            _logger.LogInformation(
                "--> BİLDİRİM GÖNDERİLİYOR: Yeni post oluşturuldu! Post ID: {PostId}",
                notification.PostId);
            // "Fire-and-forget" olduğu için bir şey döndürmemize gerek yok.
            return Task.CompletedTask;
        }

    }
}
