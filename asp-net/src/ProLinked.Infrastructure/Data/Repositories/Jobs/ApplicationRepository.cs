using Microsoft.EntityFrameworkCore;
using ProLinked.Domain;
using ProLinked.Domain.Contracts.Jobs;
using ProLinked.Domain.Extensions;
using ProLinked.Domain.Shared.Jobs;
using ProLinked.Infrastructure.Data;
using ProLinkedApplication = ProLinked.Domain.Entities.Jobs.Application;

namespace ProLinked.Infrastructure.Data.Repositories.Jobs;

public class ApplicationRepository: ProLinkedBaseRepository<ProLinkedApplication, Guid>, IApplicationRepository
{
    public ApplicationRepository(
        ProLinkedDbContext dbContext)
        : base(dbContext)
    {
        DefaultSorting = $"{nameof(ProLinkedApplication.CreationTime)} DESC";
    }

    public async Task<List<ProLinkedApplication>> GetListByUserAsync(
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

    public override async Task<IQueryable<ProLinkedApplication>> WithDetailsAsync(CancellationToken cancellationToken = default)
    {
        return (await GetQueryableAsync(cancellationToken)).
            Include(e => e.User);
    }

    private async Task<IQueryable<ProLinkedApplication>> FilterQueryableAsync(
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