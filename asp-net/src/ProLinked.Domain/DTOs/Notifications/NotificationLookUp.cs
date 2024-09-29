using ProLinked.Domain.Shared.Notifications;

namespace ProLinked.Domain.DTOs.Notifications;

public class NotificationLookUp
{
    public Guid UserId;
    public Guid SourceId;
    public string UserFullName = null!;
    public NotificationTypeEnum NotificationType;
    public string Description = null!;
}