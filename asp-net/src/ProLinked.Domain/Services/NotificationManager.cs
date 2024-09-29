using ProLinked.Domain.Contracts.Notifications;
using ProLinked.Domain.Entities.Notifications;
using ProLinked.Domain.Shared;
using ProLinked.Domain.Shared.Exceptions;
using ProLinked.Domain.Shared.Notifications;

namespace ProLinked.Domain.Services;

public class NotificationManager: INotificationManager
{
    private INotificationRepository NotificationRepository { get; init; }

    public  NotificationManager(
        INotificationRepository notificationRepository)
    {
        NotificationRepository = notificationRepository;
    }

    public async Task<Notification> CreateNotificationForPostReactionAsync(
        Guid currentUserId,
        Guid targetUserId,
        Guid postReactionId,
        CancellationToken cancellationToken = default)
    {
        var newNotification = await CreateNotificationAsync(
            currentUserId,
            targetUserId,
            postReactionId,
            NotificationTypeEnum.POST_REACTION,
            cancellationToken);

        return newNotification;
    }

    public async Task<Notification> CreateNotificationForCommentAsync(
        Guid currentUserId,
        Guid targetUserId,
        Guid commentId,
        CancellationToken cancellationToken = default)
    {
        var newNotification = await CreateNotificationAsync(
            currentUserId,
            targetUserId,
            commentId,
            NotificationTypeEnum.COMMENT,
            cancellationToken);

        return newNotification;
    }

    public async Task<Notification> CreateNotificationForCommentReactionAsync(
        Guid currentUserId,
        Guid targetUserId,
        Guid commentReactionId,
        CancellationToken cancellationToken = default)
    {
        var newNotification = await CreateNotificationAsync(
            currentUserId,
            targetUserId,
            commentReactionId,
            NotificationTypeEnum.COMMENT_REACTION,
            cancellationToken);

        return newNotification;
    }

    public async Task<Notification> CreateNotificationForConnectionRequestAsync(
        Guid currentUserId,
        Guid targetUserId,
        Guid connectionRequestId,
        CancellationToken cancellationToken = default)
    {
        var newNotification = await CreateNotificationAsync(
            currentUserId,
            targetUserId,
            connectionRequestId,
            NotificationTypeEnum.CONNECTION_REQUEST,
            cancellationToken);

        return newNotification;
    }

    public async Task<Notification> CreateNotificationForJobAdvertisementAsync(
        Guid currentUserId,
        Guid targetUserId,
        Guid jobAdvertisementId,
        CancellationToken cancellationToken = default)
    {
        var newNotification = await CreateNotificationAsync(
            currentUserId,
            targetUserId,
            jobAdvertisementId,
            NotificationTypeEnum.JOB_ADVERTISEMENT,
            cancellationToken);

        return newNotification;
    }

    public async Task<Notification> CreateNotificationForJobApplicationAsync(
        Guid currentUserId,
        Guid targetUserId,
        Guid jobApplicationId,
        CancellationToken cancellationToken = default)
    {
        var newNotification = await CreateNotificationAsync(
            currentUserId,
            targetUserId,
            jobApplicationId,
            NotificationTypeEnum.JOB_APPLICATION,
            cancellationToken);

        return newNotification;
    }

    public async Task<Notification> CreateNotificationForMessageAsync(
        Guid currentUserId,
        Guid targetUserId,
        Guid messageId,
        CancellationToken cancellationToken = default)
    {
        var newNotification = await CreateNotificationAsync(
            currentUserId,
            targetUserId,
            messageId,
            NotificationTypeEnum.MESSAGE,
            cancellationToken);

        return newNotification;
    }


    public async Task<Notification> UpdateShownStatusAsync(
        Guid currentUserId,
        Guid notificationId,
        bool isShown = true,
        CancellationToken cancellationToken = default)
    {
        var notification = await NotificationRepository.FindAsync(notificationId, false, cancellationToken);
        if (notification is null)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.NotificationNotFound);
        }
        if (notification.TargetUserId != currentUserId)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.UserNotTargetOfNotification);
        }

        cancellationToken.ThrowIfCancellationRequested();
        notification.SetIsShown(isShown);
        return notification;
    }

    private async Task<Notification> CreateNotificationAsync(
        Guid currentUserId,
        Guid targetUserId,
        Guid sourceId,
        NotificationTypeEnum notificationType,
        CancellationToken cancellationToken = default)
    {
        if (currentUserId == Guid.Empty)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.UserNotLoggedIn);
        }
        if (currentUserId == targetUserId)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.UserTargetAndCreatorOfNotification);
        }

        var notification = await NotificationRepository.FindAsync(e =>
            e.TargetUserId == targetUserId &&
            e.SourceId == sourceId &&
            e.NotificationType == notificationType &&
            e.CreatorId == currentUserId &&
            e.IsShown == false,
            false,
            cancellationToken);
        if (notification is not null)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.NotificationAlreadyExists);
        }

        cancellationToken.ThrowIfCancellationRequested();
        var newNotification = new Notification(
            Guid.NewGuid(),
            currentUserId,
            targetUserId,
            sourceId,
            notificationType);
        await NotificationRepository.InsertAsync(newNotification, autoSave: true, cancellationToken);

        return newNotification;
    }
}