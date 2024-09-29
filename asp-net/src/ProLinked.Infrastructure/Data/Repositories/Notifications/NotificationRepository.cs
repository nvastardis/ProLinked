using Microsoft.EntityFrameworkCore;
using ProLinked.Domain;
using ProLinked.Domain.Contracts.Notifications;
using ProLinked.Domain.DTOs.Notifications;
using ProLinked.Domain.Entities.Identity;
using ProLinked.Domain.Entities.Notifications;
using ProLinked.Domain.Shared.Utils;

namespace ProLinked.Infrastructure.Data.Repositories.Notifications;

public class NotificationRepository: ProLinkedBaseRepository<Notification, Guid>, INotificationRepository
{
    public NotificationRepository(
        ProLinkedDbContext dbContext)
        : base(dbContext)
    {
        DefaultSorting = $"{nameof(Notification.CreationTime)} DESC";
    }

    public async Task<List<NotificationLookUp>> GetListByUserAsync(
        Guid userId,
        int skipCount = ProLinkedConsts.SkipCountDefaultValue,
        int maxResultCount = ProLinkedConsts.MaxResultCountDefaultValue,
        CancellationToken cancellationToken = default)
    {
        var userQueryable = (await GetDbContextAsync(cancellationToken)).Set<AppUser>();
        var filteredQuery = await FilteredQueryAsync(userId, cancellationToken:cancellationToken);
        var paginatedQuery = ApplyPagination(filteredQuery, null, skipCount, maxResultCount);

        var fullQuery =
            from notification in paginatedQuery
            join user in userQueryable on notification.TargetUserId equals user.Id
            select new NotificationLookUp()
            {
                UserId = user.Id,
                SourceId = notification.SourceId,
                UserFullName = $"{user.Name} {user.Surname}",
                NotificationType = notification.NotificationType,
                Description = $"New {notification.NotificationType.ToString().Replace('_',' ')} from: {user.Name} {user.Surname}"
            };

        return await fullQuery.ToListAsync(cancellationToken);
    }

    private async Task<IQueryable<Notification>> FilteredQueryAsync(
        Guid? userId = null,
        bool? isShown = null,
        DateTime? from = null,
        DateTime? to = null,
        bool includeDetails = false,
        CancellationToken cancellationToken = default)
    {
        var queryable = includeDetails ? await WithDetailsAsync(cancellationToken) : await GetQueryableAsync(cancellationToken);

        var query = queryable.
            WhereIf(
                userId.HasValue,
                c => c.TargetUserId == userId).
            WhereIf(
                isShown.HasValue,
                c => c.IsShown == isShown).
            WhereIf(
                from.HasValue,
                c => c.CreationTime >= from).
            WhereIf(
                to.HasValue,
                c => c.CreationTime <= to);
        return query;
    }
}