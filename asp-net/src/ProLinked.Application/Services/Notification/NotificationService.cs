using AutoMapper;
using ProLinked.Application.Contracts.Notifications;
using ProLinked.Application.Contracts.Notifications.DTOs;
using ProLinked.Application.DTOs;
using ProLinked.Domain;
using ProLinked.Domain.Contracts.Notifications;
using ProLinked.Domain.DTOs.Notifications;

namespace ProLinked.Application.Services.Notification;

public class NotificationService: ProLinkedServiceBase, INotificationService
{
    private INotificationManager NotificationManager;
    private INotificationRepository NotificationRepository;

    public NotificationService(
        IMapper mapper,
        INotificationManager notificationManager,
        INotificationRepository notificationRepository)
        : base(mapper)
    {
        NotificationManager = notificationManager;
        NotificationRepository = notificationRepository;
    }


    public async Task<PagedAndSortedResultList<NotificationLookUpDto>> GetNotificationList(
        Guid userId,
        int skipCount = ProLinkedConsts.SkipCountDefaultValue,
        int maxResultCount = ProLinkedConsts.MaxResultCountDefaultValue,
        CancellationToken cancellationToken = default)
    {
        var queryResult = await NotificationRepository.GetListByUserAsync(
            userId,
            skipCount,
            maxResultCount,
            cancellationToken);

        var items = ObjectMapper.Map<List<NotificationLookUp>, List<NotificationLookUpDto>>(queryResult);
        var itemCount = items.Count;
        return new PagedAndSortedResultList<NotificationLookUpDto>(itemCount, items);
    }

    public async Task UpdateShownStatusAsync(
        Guid userId,
        Guid notificationId,
        CancellationToken cancellationToken = default)
    {
        await NotificationManager.UpdateShownStatusAsync(
            userId,
            notificationId,
            true,
            cancellationToken);
    }
}