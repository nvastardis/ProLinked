using ProLinked.Domain.Entities.Notifications;

namespace ProLinked.Domain.Contracts.Notifications;

public interface INotificationManager
{
    Task<Notification> CreateNotificationForPostReactionAsync(
        Guid currentUserId,
        Guid targetUserId,
        Guid postReactionId,
        CancellationToken cancellationToken = default);

    Task<Notification> CreateNotificationForCommentAsync(
        Guid currentUserId,
        Guid targetUserId,
        Guid commentId,
        CancellationToken cancellationToken = default);

    Task<Notification> CreateNotificationForCommentReactionAsync(
        Guid currentUserId,
        Guid targetUserId,
        Guid commentReactionId,
        CancellationToken cancellationToken = default);

    Task<Notification> CreateNotificationForConnectionRequestAsync(
        Guid currentUserId,
        Guid targetUserId,
        Guid connectionRequestId,
        CancellationToken cancellationToken = default);

    Task<Notification> CreateNotificationForJobAdvertisementAsync(
        Guid currentUserId,
        Guid targetUserId,
        Guid jobAdvertisementId,
        CancellationToken cancellationToken = default);

    Task<Notification> CreateNotificationForJobApplicationAsync(
        Guid currentUserId,
        Guid targetUserId,
        Guid jobApplicationId,
        CancellationToken cancellationToken = default);

    Task<Notification> CreateNotificationForMessageAsync(
        Guid currentUserId,
        Guid targetUserId,
        Guid messageId,
        CancellationToken cancellationToken = default);

    Task<Notification> UpdateShownStatusAsync(
        Guid currentUserId,
        Guid notificationId,
        bool isShown = true,
        CancellationToken cancellationToken = default);
}