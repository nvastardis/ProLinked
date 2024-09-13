using System.Linq.Dynamic.Core;
using ProLinked.Domain.Entities;
using ProLinked.Domain.Shared;
using ProLinked.Infrastructure.Data;

namespace ProLinked.Infrastructure.Repositories;


public abstract class ProLinkedBaseRepository<TEntity> : EfCoreRepository<ProLinkedDbContext, TEntity>
    where TEntity: Entity
{
    protected string DefaultSorting { get; init; } = string.Empty;

    protected ProLinkedBaseRepository(
        ProLinkedDbContext dbContext)
        : base(dbContext)
    {
    }

    protected virtual IQueryable<TEntity> ApplyPagination(
        IQueryable<TEntity> query,
        string? sorting,
        int skipCount = ProLinkedConsts.SkipCountDefaultValue,
        int maxResultCount = ProLinkedConsts.MaxResultCountDefaultValue)
    {
        var sortingQuery = sorting ?? DefaultSorting;

        query = query.OrderBy(sortingQuery);

        return query.Page(skipCount, maxResultCount);
    }
}


public abstract class ProLinkedBaseRepository<TEntity, TKey> : EfCoreRepository<ProLinkedDbContext, TEntity, TKey>
    where TEntity: Entity<TKey>
{
    protected string DefaultSorting { get; init; } = string.Empty;

    protected ProLinkedBaseRepository(
        ProLinkedDbContext dbContext)
        : base(dbContext)
    {
    }

    protected virtual IQueryable<TEntity> ApplyPagination(
        IQueryable<TEntity> query,
        string? sorting,
        int skipCount = ProLinkedConsts.SkipCountDefaultValue,
        int maxResultCount = ProLinkedConsts.MaxResultCountDefaultValue)
    {
        var sortingQuery = sorting ?? DefaultSorting;

        query = query.OrderBy(sortingQuery);

        return query.Page(skipCount, maxResultCount);
    }
}
