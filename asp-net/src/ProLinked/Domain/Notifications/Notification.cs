using ProLinked.Shared.Notifications;

namespace ProLinked.Domain.Notifications;

public class Notification: Entity<Guid>
{
    public Guid CreatorId { get; init; }
    public Guid TargetUserId { get; init; }
    public Guid SourceId { get; init; }
    public NotificationTypeEnum NotificationType { get; init; }
    public bool IsShown { get; private set; }
    public DateTime CreationTime { get; init; }

    private Notification(Guid id): base(id){}

    internal Notification(
        Guid id,
        Guid creatorId,
        Guid targetUserId,
        Guid sourceId,
        NotificationTypeEnum notificationType)
        : base(id)
    {
        CreatorId = creatorId;
        TargetUserId = targetUserId;
        SourceId = sourceId;
        NotificationType = notificationType;
        SetIsShown();
        CreationTime = DateTime.Now;
    }

    internal Notification SetIsShown(bool isShown = false)
    {
        IsShown = isShown;
        return this;
    }
}
