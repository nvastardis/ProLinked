using Microsoft.EntityFrameworkCore;
using ProLinked.Domain;
using ProLinked.Domain.Contracts.Notifications;
using ProLinked.Domain.Entities.Notifications;
using ProLinked.Domain.Extensions;
using ProLinked.Domain.Shared.Utils;
using ProLinked.Infrastructure.Data;

namespace ProLinked.Infrastructure.Data.Repositories.Notifications;

public class NotificationRepository: ProLinkedBaseRepository<Notification, Guid>, INotificationRepository
{
    public NotificationRepository(
        ProLinkedDbContext dbContext)
        : base(dbContext)
    {
        DefaultSorting = $"{nameof(Notification.CreationTime)} DESC";
    }

    public async Task<List<Notification>> GetListByUserAsync(
        Guid userId,
        bool? isShown = null,
        DateTime? from = null,
        DateTime? to = null,
        bool includeDetails = false,
        string? sorting = null,
        int skipCount = ProLinkedConsts.SkipCountDefaultValue,
        int maxResultCount = ProLinkedConsts.MaxResultCountDefaultValue,
        CancellationToken cancellationToken = default)
    {
        var filteredQuery = await FilteredQueryAsync(userId, isShown, from, to, includeDetails, cancellationToken);
        var paginatedQuery = ApplyPagination(filteredQuery, sorting, skipCount, maxResultCount);
        return await paginatedQuery.ToListAsync(cancellationToken);
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