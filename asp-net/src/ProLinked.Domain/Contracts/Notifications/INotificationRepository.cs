using ProLinked.Domain.DTOs.Notifications;
using ProLinked.Domain.Entities.Notifications;

namespace ProLinked.Domain.Contracts.Notifications;

public interface INotificationRepository: IRepository<Notification, Guid>
{
    Task<List<NotificationLookUp>> GetListByUserAsync(
        Guid userId,
        int skipCount = ProLinkedConsts.SkipCountDefaultValue,
        int maxResultCount = ProLinkedConsts.MaxResultCountDefaultValue,
        CancellationToken cancellationToken = default);
}