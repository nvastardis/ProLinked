using ProLinked.Application.Contracts.Notifications.DTOs;
using ProLinked.Domain;

namespace ProLinked.Application.Contracts.Notifications;

public interface INotificationService
{
    Task<List<NotificationLookUpDto>> GetNotificationList(
        Guid userId,
        int skipCount = ProLinkedConsts.SkipCountDefaultValue,
        int maxResultCount = ProLinkedConsts.MaxResultCountDefaultValue,
        CancellationToken cancellationToken = default);
    Task UpdateShownStatusAsync(
        Guid userId,
        Guid notificationId,
        bool isShown = true,
        CancellationToken cancellationToken = default);

}