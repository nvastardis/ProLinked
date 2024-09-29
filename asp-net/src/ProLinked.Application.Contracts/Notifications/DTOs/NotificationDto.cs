using ProLinked.Domain.Shared.Notifications;

namespace ProLinked.Application.Contracts.Notifications.DTOs;

public class NotificationDto
{
    public Guid CreatorId { get; init; }
    public Guid TargetUserId { get; init; }
    public Guid SourceId { get; init; }
    public NotificationTypeEnum NotificationType { get; init; }
}