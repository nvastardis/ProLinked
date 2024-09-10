using Microsoft.EntityFrameworkCore;
using ProLinked.Domain.Jobs;
using ProLinked.Shared;
using ProLinked.Shared.Extensions;
using ProLinked.Shared.Jobs;
using ProLinked.Shared.Utils;

namespace ProLinked.Data.Jobs;

public class AdvertisementRepository: ProLinkedBaseRepository<Advertisement, Guid>, IAdvertisementRepository
{
    public AdvertisementRepository(
        ProLinkedDbContext dbContext)
        : base(dbContext)
    {
        DefaultSorting = $"{nameof(Advertisement.CreationTime)} DESC";
    }

    public async Task<List<Advertisement>> GetListByUserAsync(
        Guid userId,
        DateTime? from = null,
        DateTime? to = null,
        AdvertisementStatus status = AdvertisementStatus.UNDEFINED,
        bool includeDetails = false,
        string? sorting = null,
        int skipCount = ProLinkedConsts.SkipCountDefaultValue,
        int maxResultCount = ProLinkedConsts.MaxResultCountDefaultValue,
        CancellationToken cancellationToken = default)
    {
        var filteredQueryable = await FilterQueryableAsync(userId, from, to, status, includeDetails);
        filteredQueryable = ApplyPagination(filteredQueryable, sorting, skipCount, maxResultCount);
        return await filteredQueryable.ToListAsync(cancellationToken);
    }

    public override async Task<IQueryable<Advertisement>> WithDetailsAsync(CancellationToken cancellationToken = default)
    {
        return (await GetQueryableAsync(cancellationToken)).
            Include(e => e.Applications);
    }

    private async Task<IQueryable<Advertisement>> FilterQueryableAsync(
        Guid? userId,
        DateTime? from = null,
        DateTime? to = null,
        AdvertisementStatus status = AdvertisementStatus.UNDEFINED,
        bool includeDetails = false,
        CancellationToken cancellationToken = default)
    {
        var queryable = includeDetails ? await WithDetailsAsync(cancellationToken) : await GetQueryableAsync(cancellationToken);

        var query = queryable
            .WhereIf(
                userId.HasValue,
                c => c.CreatorId == userId)
            .WhereIf(
                from.HasValue,
                c => c.CreationTime >= from)
            .WhereIf(
                to.HasValue,
                c => c.CreationTime <= to)
            .WhereIf(
                status != AdvertisementStatus.UNDEFINED,
                c => c.Status == status);

        return query;
    }
}
