using ProLinked.Shared;
using ProLinked.Shared.Exceptions;
using ProLinked.Shared.Notifications;

namespace ProLinked.Domain.Notifications;

public class NotificationManager: IDomainService
{
    private INotificationRepository NotificationRepository { get; init; }

    public  NotificationManager(
        INotificationRepository notificationRepository)
    {
        NotificationRepository = notificationRepository;
    }

    public async Task<Notification> CreateNotificationForPostReaction(
        Guid currentUserId,
        Guid targetUserId,
        Guid postReactionId)
    {
        var newNotification = await CreateNotification(
            currentUserId,
            targetUserId,
            postReactionId,
            NotificationTypeEnum.POST_REACTION);

        return newNotification;
    }

    public async Task<Notification> CreateNotificationForComment(
        Guid currentUserId,
        Guid targetUserId,
        Guid commentId)
    {
        var newNotification = await CreateNotification(
            currentUserId,
            targetUserId,
            commentId,
            NotificationTypeEnum.COMMENT);

        return newNotification;
    }

    public async Task<Notification> CreateNotificationForCommentReaction(
        Guid currentUserId,
        Guid targetUserId,
        Guid commentReactionId)
    {
        var newNotification = await CreateNotification(
            currentUserId,
            targetUserId,
            commentReactionId,
            NotificationTypeEnum.COMMENT_REACTION);

        return newNotification;
    }

    public async Task<Notification> CreateNotificationForConnectionRequest(
        Guid currentUserId,
        Guid targetUserId,
        Guid connectionRequestId)
    {
        var newNotification = await CreateNotification(
            currentUserId,
            targetUserId,
            connectionRequestId,
            NotificationTypeEnum.CONNECTION_REQUEST);

        return newNotification;
    }

    public async Task<Notification> CreateNotificationForJobAdvertisement(
        Guid currentUserId,
        Guid targetUserId,
        Guid jobAdvertisementId)
    {
        var newNotification = await CreateNotification(
            currentUserId,
            targetUserId,
            jobAdvertisementId,
            NotificationTypeEnum.JOB_ADVERTISEMENT);

        return newNotification;
    }

    public async Task<Notification> CreateNotificationForJobApplication(
        Guid currentUserId,
        Guid targetUserId,
        Guid jobApplicationId)
    {
        var newNotification = await CreateNotification(
            currentUserId,
            targetUserId,
            jobApplicationId,
            NotificationTypeEnum.JOB_APPLICATION);

        return newNotification;
    }

    public async Task<Notification> CreateNotificationForMessage(
        Guid currentUserId,
        Guid targetUserId,
        Guid messageId)
    {
        var newNotification = await CreateNotification(
            currentUserId,
            targetUserId,
            messageId,
            NotificationTypeEnum.MESSAGE);

        return newNotification;
    }


    public async Task<Notification> UpdateShownStatusAsync(
        Guid currentUserId,
        Guid notificationId,
        bool isShown = true)
    {
        var notification = await NotificationRepository.FindAsync(notificationId);
        if (notification is null)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.NotificationNotFound);
        }
        if (notification.TargetUserId != currentUserId)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.UserNotTargetOfNotification);
        }

        notification.SetIsShown(isShown);
        return notification;
    }

    private async Task<Notification> CreateNotification(
        Guid currentUserId,
        Guid targetUserId,
        Guid sourceId,
        NotificationTypeEnum notificationType)
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
            e.IsShown == false);
        if (notification is not null)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.NotificationAlreadyExists);
        }

        var newNotification = new Notification(
            Guid.NewGuid(),
            currentUserId,
            targetUserId,
            sourceId,
            notificationType);

        return newNotification;
    }
}
