﻿using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProLinked.Domain;
using ProLinked.Domain.Entities;
using ProLinked.Domain.Shared.Exceptions;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace ProLinked.Infrastructure.Data.Repositories;

public class EfCoreRepository<TDbContext, TEntity>: IRepository<TEntity>
    where TDbContext: DbContext
    where TEntity: Entity
{
    protected readonly TDbContext DbContext;
    protected EfCoreRepository(
        TDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public async Task<DbContext> GetDbContextAsync(CancellationToken cancellationToken = default)
    {
        return await Task.Run(() => DbContext, cancellationToken);
    }

    public async Task<DbSet<TEntity>> GetDbSetAsync(CancellationToken cancellationToken = default)
    {
        return await Task.Run(() => DbContext.Set<TEntity>(), cancellationToken);
    }

    public async Task<TEntity> InsertAsync(
        TEntity entity,
        bool autoSave = true,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync(cancellationToken);
        var savedEntity = (await dbSet.AddAsync(entity, cancellationToken)).Entity;
        if (autoSave)
        {
            await DbContext.SaveChangesAsync(cancellationToken);
        }
        return savedEntity;
    }

    public async Task InsertManyAsync(
        IEnumerable<TEntity> entities,
        bool autoSave = true,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync(cancellationToken);
        await dbSet.AddRangeAsync(entities, cancellationToken);
        if (autoSave)
        {
            await DbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<TEntity?> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        bool includeDetails = true,
        CancellationToken cancellationToken = default)
    {
        return includeDetails
            ? await (await WithDetailsAsync(cancellationToken))
                .Where(predicate)
                .SingleOrDefaultAsync(cancellationToken)
            : await (await GetQueryableAsync(cancellationToken))
                .Where(predicate)
                .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<TEntity>?> FindManyAsync(
        Expression<Func<TEntity, bool>> predicate,
        bool includeDetails = true,
        CancellationToken cancellationToken = default)
    {
        return includeDetails
            ? await (await WithDetailsAsync(cancellationToken))
                .Where(predicate)
                .ToListAsync(cancellationToken)
            : await (await GetQueryableAsync(cancellationToken))
                .Where(predicate)
                .ToListAsync(cancellationToken);
    }

    public async Task<TEntity> GetAsync(
        Expression<Func<TEntity, bool>> predicate,
        bool includeDetails = true,
        CancellationToken cancellationToken = default)
    {
        var entity = await FindAsync(predicate, includeDetails, cancellationToken);

        if (entity == null)
        {
            throw new EntityNotFoundException(typeof(TEntity));
        }

        return entity;
    }

    public async Task<List<TEntity>> GetListAsync(
        bool includeDetails = false,
        CancellationToken cancellationToken = default)
    {
        return includeDetails
            ? await (await WithDetailsAsync(cancellationToken)).ToListAsync(cancellationToken)
            : await (await GetQueryableAsync(cancellationToken)).ToListAsync(cancellationToken);
    }

    public async Task<long> GetCountAsync(
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync(cancellationToken);
        return await dbSet.AsQueryable().LongCountAsync(cancellationToken);
    }

    public async Task<List<TEntity>> GetPagedListAsync(
        int skipCount,
        int maxResultCount,
        string? sorting,
        bool includeDetails = false,
        CancellationToken cancellationToken = default)
    {
        var queryable = includeDetails
            ? await WithDetailsAsync(cancellationToken)
            : await GetQueryableAsync(cancellationToken);

        return await queryable
            .OrderBy(sorting ?? string.Empty)
            .Page(skipCount, maxResultCount)
            .ToListAsync(cancellationToken);
    }

    public async Task<TEntity> UpdateAsync(
        TEntity entity,
        bool autoSave = true,
        CancellationToken cancellationToken = default)
    {
        if (DbContext.Set<TEntity>().Local.All(e => e != entity))
        {
            DbContext.Set<TEntity>().Attach(entity);
            DbContext.Update(entity);
        }

        if (autoSave)
        {
            await DbContext.SaveChangesAsync(cancellationToken);
        }

        return entity;
    }

    public async Task UpdateManyAsync(
        IEnumerable<TEntity> entities,
        bool autoSave = true,
        CancellationToken cancellationToken = default)
    {
        var enumerable = entities as TEntity[] ?? [];
        if (enumerable.IsNullOrEmpty())
        {
            return;
        }

        var dbSet = await GetDbSetAsync(cancellationToken);
        dbSet.UpdateRange(enumerable);

        if (autoSave)
        {
            await DbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task DeleteAsync(
        TEntity entity,
        bool autoSave = true,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync(cancellationToken);
        dbSet.Remove(entity);
        if (autoSave)
        {
            await DbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task DeleteManyAsync(
        Expression<Func<TEntity, bool>> predicate,
        bool autoSave = true,
        CancellationToken cancellationToken = default)
    {
        var items = await FindManyAsync(predicate, false, cancellationToken);
        var enumerable = items as TEntity[] ?? [];
        if (enumerable.IsNullOrEmpty())
        {
            return;
        }

        await DeleteManyAsync(enumerable, autoSave, cancellationToken);
    }

    public async Task DeleteManyAsync(
        IEnumerable<TEntity> entities,
        bool autoSave = true,
        CancellationToken cancellationToken = default)
    {
        var enumerable = entities.ToArray();
        if (enumerable.IsNullOrEmpty())
        {
            return;
        }

        var dbSet = await GetDbSetAsync(cancellationToken);
        dbSet.RemoveRange(enumerable);

        if (autoSave)
        {
            await DbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public virtual async Task<IQueryable<TEntity>> WithDetailsAsync(CancellationToken cancellationToken = default)
    {
        return await GetQueryableAsync(cancellationToken);
    }

    public virtual async Task<IQueryable<TEntity>> WithDetailsAsync(Expression<Func<TEntity, object>>[] propertySelectors, CancellationToken cancellationToken = default)
    {
        return await GetQueryableAsync(cancellationToken);
    }

    public virtual async Task<IQueryable<TEntity>> GetQueryableAsync(CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync(cancellationToken);
        return await Task.FromResult(dbSet.AsQueryable());
    }
}

public class EfCoreRepository<TDbContext, TEntity, TKey> : EfCoreRepository<TDbContext,TEntity>, IRepository<TEntity, TKey>
    where TEntity : Entity<TKey>
    where TDbContext: DbContext
{
    protected EfCoreRepository(
        TDbContext dbContext)
        : base(dbContext)
    {

    }
    public async Task<TEntity> GetAsync(
        TKey id,
        bool includeDetails = true,
        CancellationToken cancellationToken = default)
    {
        var entity = await FindAsync(id, includeDetails, cancellationToken);

        if (entity == null)
        {
            throw new EntityNotFoundException(typeof(TEntity), id);
        }

        return entity;
    }

    public async Task<TEntity?> FindAsync(
        TKey id,
        bool includeDetails = true,
        CancellationToken cancellationToken = default)
    {
        return includeDetails
            ? await (await WithDetailsAsync(cancellationToken)).
                OrderBy(e => e.Id).
                FirstOrDefaultAsync(e => e.Id!.Equals(id), cancellationToken)
            : await (await GetQueryableAsync(cancellationToken)).
                OrderBy(e => e.Id).
                FirstOrDefaultAsync(e => e.Id!.Equals(id), cancellationToken);
    }

    public async Task<bool> DeleteAsync(TKey id, bool autoSave = true, CancellationToken cancellationToken = default)
    {
        var entity = await FindAsync(id, cancellationToken: cancellationToken);
        if (entity == null)
        {
            return false;
        }

        await DeleteAsync(entity, autoSave, cancellationToken);
        return true;
    }

    public async Task DeleteManyAsync(
        IEnumerable<TKey> ids,
        bool autoSave = true,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync(cancellationToken);
        var entities =
            dbSet.
                AsQueryable().
                Where(x => ids.Contains(x.Id)).
                AsEnumerable();

        await DeleteManyAsync(entities, autoSave, cancellationToken);
    }
}