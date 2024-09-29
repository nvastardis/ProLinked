using Microsoft.AspNetCore.SignalR;
using ProLinked.Domain.Contracts.Notifications;

namespace ProLinked.API.Controllers.Notifications;

public class NotificationHub: Hub
{
    private readonly INotificationManager _notificationManager;

    public NotificationHub(INotificationManager notificationManager)
    {
        _notificationManager = notificationManager;
    }

    public async Task CreateNotificationForPostReactionAsync(
        Guid currentUserId,
        Guid targetUserId,
        Guid postReactionId,
        CancellationToken cancellationToken = default)
    {
        var newNotification = await _notificationManager.CreateNotificationForPostReactionAsync(
            currentUserId,
            targetUserId,
            postReactionId,
            cancellationToken);

        await Clients.User(targetUserId.ToString()).SendAsync("ReceiveNotification", newNotification, cancellationToken);
    }

    public async Task CreateNotificationForCommentAsync(
        Guid currentUserId,
        Guid targetUserId,
        Guid commentId,
        CancellationToken cancellationToken = default)
    {
        var newNotification = await _notificationManager.CreateNotificationForCommentAsync(
            currentUserId,
            targetUserId,
            commentId,
            cancellationToken);

        await Clients.User(targetUserId.ToString()).SendAsync("ReceiveNotification", newNotification, cancellationToken);
    }

    public async Task CreateNotificationForCommentReactionAsync(
        Guid currentUserId,
        Guid targetUserId,
        Guid commentReactionId,
        CancellationToken cancellationToken = default)
    {
        var newNotification = await _notificationManager.CreateNotificationForCommentReactionAsync(
            currentUserId,
            targetUserId,
            commentReactionId,
            cancellationToken);

        await Clients.User(targetUserId.ToString()).SendAsync("ReceiveNotification", newNotification, cancellationToken);
    }

    public async Task CreateNotificationForConnectionRequestAsync(
        Guid currentUserId,
        Guid targetUserId,
        Guid connectionRequestId,
        CancellationToken cancellationToken = default)
    {
        var newNotification = await _notificationManager.CreateNotificationForConnectionRequestAsync(
            currentUserId,
            targetUserId,
            connectionRequestId,
            cancellationToken);

        await Clients.User(targetUserId.ToString()).SendAsync("ReceiveNotification", newNotification, cancellationToken);
    }

    public async Task CreateNotificationForJobAdvertisementAsync(
        Guid currentUserId,
        Guid targetUserId,
        Guid jobAdvertisementId,
        CancellationToken cancellationToken = default)
    {
        var newNotification = await _notificationManager.CreateNotificationForJobAdvertisementAsync(
            currentUserId,
            targetUserId,
            jobAdvertisementId,
            cancellationToken);

        await Clients.User(targetUserId.ToString()).SendAsync("ReceiveNotification", newNotification, cancellationToken);
    }

    public async Task CreateNotificationForJobApplicationAsync(
        Guid currentUserId,
        Guid targetUserId,
        Guid jobApplicationId,
        CancellationToken cancellationToken = default)
    {
        var newNotification = await _notificationManager.CreateNotificationForJobApplicationAsync(
            currentUserId,
            targetUserId,
            jobApplicationId,
            cancellationToken);

        await Clients.User(targetUserId.ToString()).SendAsync("ReceiveNotification", newNotification, cancellationToken);
    }

    public async Task CreateNotificationForMessageAsync(
        Guid currentUserId,
        Guid targetUserId,
        Guid messageId,
        CancellationToken cancellationToken = default)
    {
        var newNotification = await _notificationManager.CreateNotificationForMessageAsync(
            currentUserId,
            targetUserId,
            messageId,
            cancellationToken);

        await Clients.User(targetUserId.ToString()).SendAsync("ReceiveMessage", newNotification, cancellationToken);
    }
}