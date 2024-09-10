using Microsoft.EntityFrameworkCore;
using ProLinked.Domain.Jobs;
using ProLinked.Shared;
using ProLinked.Shared.Extensions;
using ProLinked.Shared.Jobs;

namespace ProLinked.Data.Jobs;

public class ApplicationRepository: ProLinkedBaseRepository<Application, Guid>, IApplicationRepository
{
    public ApplicationRepository(
        ProLinkedDbContext dbContext)
        : base(dbContext)
    {
        DefaultSorting = $"{nameof(Application.CreationTime)} DESC";
    }

    public async Task<List<Application>> GetListByUserAsync(
        Guid userId,
        DateTime? from = null,
        DateTime? to = null,
        ApplicationStatus status = ApplicationStatus.UNDEFINED,
        bool includeDetails = false,
        string? sorting = null,
        int skipCount = ProLinkedConsts.SkipCountDefaultValue,
        int maxResultCount = ProLinkedConsts.MaxResultCountDefaultValue,
        CancellationToken cancellationToken = default)
    {
        var filteredQueryable = await FilterQueryableAsync(userId, from, to, status, includeDetails, cancellationToken);
        filteredQueryable = ApplyPagination(filteredQueryable, sorting, skipCount, maxResultCount);
        return await filteredQueryable.ToListAsync(cancellationToken);
    }

    public override async Task<IQueryable<Application>> WithDetailsAsync(CancellationToken cancellationToken = default)
    {
        return (await GetQueryableAsync(cancellationToken)).
            Include(e => e.User);
    }

    private async Task<IQueryable<Application>> FilterQueryableAsync(
        Guid? userId,
        DateTime? from = null,
        DateTime? to = null,
        ApplicationStatus status = ApplicationStatus.UNDEFINED,
        bool includeDetails = false,
        CancellationToken cancellationToken = default)
    {
        var queryable = includeDetails ? await WithDetailsAsync(cancellationToken) : await GetQueryableAsync(cancellationToken);

        var query = queryable.
                    WhereIf(
                        userId.HasValue,
                        c => c.UserId == userId).
                    WhereIf(
                        from.HasValue,
                        c => c.CreationTime >= from).
                    WhereIf(
                        to.HasValue,
                        c => c.CreationTime <= to).
                    WhereIf(
                        status != ApplicationStatus.UNDEFINED,
                        c => c.Status == status);

        return query;
    }
}
