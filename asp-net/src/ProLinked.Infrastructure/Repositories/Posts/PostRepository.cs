using Microsoft.EntityFrameworkCore;
using ProLinked.Application.Services.Posts;
using ProLinked.Domain.Entities.Posts;
using ProLinked.Domain.Extensions;
using ProLinked.Domain.Shared;
using ProLinked.Infrastructure.Data;

namespace ProLinked.Infrastructure.Repositories.Posts;

public class PostRepository : ProLinkedBaseRepository<Post, Guid>, IPostRepository
{
    public PostRepository(
        ProLinkedDbContext dbContext)
        : base(dbContext)
    {
        DefaultSorting = $"{nameof(Post.CreationTime)} DESC";
    }

    public async Task<List<Post>> GetListByUserAsync(
        Guid userId,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        bool includeDetails = false,
        string? sorting = null,
        int skipCount = ProLinkedConsts.SkipCountDefaultValue,
        int maxResultCount = ProLinkedConsts.MaxResultCountDefaultValue,
        CancellationToken cancellationToken = default)
    {
        var filteredQuery = await FilterQueryableAsync(
            userId,
            fromDate,
            toDate,
            includeDetails,
            cancellationToken);

        filteredQuery = ApplyPagination(
            filteredQuery,
            sorting,
            skipCount,
            maxResultCount);

        return await filteredQuery.ToListAsync(cancellationToken);
    }

    public override async Task<IQueryable<Post>> WithDetailsAsync(CancellationToken cancellationToken = default)
    {
        var queryable = (await GetQueryableAsync(cancellationToken))
            .Include(e => e.Media)
            .Include(e => e.Reactions)
            .Include(e => e.Comments)
            .ThenInclude(e => e.Reactions);
        return queryable;
    }

    private async Task<IQueryable<Post>> FilterQueryableAsync(
        Guid? userId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        bool includeDetails = false,
        CancellationToken cancellationToken = default)
    {
        var queryable = includeDetails ? await WithDetailsAsync(cancellationToken) : await GetQueryableAsync(cancellationToken);

        var query = queryable
            .WhereIf(
                userId.HasValue,
                post => post.CreatorId == userId)
            .WhereIf(
                fromDate.HasValue,
                post => post.CreationTime >= fromDate)
            .WhereIf(
                toDate.HasValue,
                post => post.CreationTime <= toDate);

        return query;
    }
}
