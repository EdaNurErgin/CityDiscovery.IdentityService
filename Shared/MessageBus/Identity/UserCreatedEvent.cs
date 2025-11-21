using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.MessageBus.Identity
{
    public record UserCreatedEvent(
        Guid UserId,
        string UserName,
        string Email,
        string Role,
        DateTime CreatedAtUtc
    );
}
