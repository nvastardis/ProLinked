using ProLinked.Domain;
using ProLinked.Domain.Entities;
using ProLinked.Infrastructure.Data;
using System.Linq.Dynamic.Core;

namespace ProLinked.Infrastructure.Data.Repositories;


public class ProLinkedBaseRepository<TEntity> : EfCoreRepository<ProLinkedDbContext, TEntity>
    where TEntity: Entity
{
    protected string DefaultSorting { get; } = string.Empty;

    public ProLinkedBaseRepository(
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


public class ProLinkedBaseRepository<TEntity, TKey> : EfCoreRepository<ProLinkedDbContext, TEntity, TKey>
    where TEntity: Entity<TKey>
{
    protected string DefaultSorting { get; init; } = string.Empty;

    public ProLinkedBaseRepository(
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