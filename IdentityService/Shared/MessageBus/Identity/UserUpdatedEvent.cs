namespace IdentityService.Shared.MessageBus.Identity
{
    public record UserUpdatedEvent(
        Guid UserId,
        string NewUserName,
        string NewAvatarUrl
    );
}