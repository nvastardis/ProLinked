using ProLinked.Domain.Entities.Notifications;

namespace ProLinked.Domain.Contracts.Notifications;

public interface INotificationRepository: IRepository<Notification, Guid>
{
    Task<List<Notification>> GetListByUserAsync(
        Guid userId,
        bool? isShown = null,
        DateTime? from = null,
        DateTime? to = null,
        bool includeDetails = false,
        string? sorting = null,
        int skipCount = ProLinkedConsts.SkipCountDefaultValue,
        int maxResultCount = ProLinkedConsts.MaxResultCountDefaultValue,
        CancellationToken cancellationToken = default);
}