using ProLinked.Application.Contracts.Notifications;
using ProLinked.Application.Contracts.Notifications.DTOs;
using ProLinked.Domain;

namespace ProLinked.Application.Services.Notification
{
    public class NotificationService: INotificationService
    {
        public Task<List<NotificationLookUpDto>> GetNotificationList(
            Guid userId,
            int skipCount = ProLinkedConsts.SkipCountDefaultValue,
            int maxResultCount = ProLinkedConsts.MaxResultCountDefaultValue,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task UpdateShownStatusAsync(Guid userId, Guid notificationId, bool isShown = true,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}