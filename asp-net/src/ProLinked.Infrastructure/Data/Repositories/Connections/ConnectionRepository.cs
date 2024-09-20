using Microsoft.EntityFrameworkCore;
using ProLinked.Domain;
using ProLinked.Domain.Contracts.Connections;
using ProLinked.Domain.DTOs.Connections;
using ProLinked.Domain.Entities.Connections;
using System.Linq.Dynamic.Core;
using ProLinked.Domain.Extensions;

namespace ProLinked.Infrastructure.Data.Repositories.Connections;

public class ConnectionRepository: ProLinkedBaseRepository<Connection, Guid>, IConnectionRepository
{
    public ConnectionRepository(
        ProLinkedDbContext dbContext)
        : base(dbContext)
    {
        DefaultSorting = $"{nameof(Connection.CreationTime)} DESC";
    }

    public async Task<List<ConnectionLookUp>> GetListByUserAsync(
        Guid userId,
        string? sorting = null,
        int skipCount = ProLinkedConsts.SkipCountDefaultValue,
        int maxResultCount = ProLinkedConsts.MaxResultCountDefaultValue,
        CancellationToken cancellationToken = default)
    {
        var connectionQueryable = await WithDetailsAsync(cancellationToken);
        var query =
            from connection in connectionQueryable
            where (connection.UserAId == userId || connection.UserBId == userId)
            select new ConnectionLookUp()
            {
                Id = connection.Id,
                UserId =
                    connection.UserAId == userId ? connection.UserBId : connection.UserAId,
                UserFullName =
                    connection.UserAId == userId
                        ? $"{connection.UserB.Name} {connection.UserB.Surname}"
                        : $"{connection.UserA.Name} {connection.UserA.Surname}",
                CreationTime = connection.CreationTime,
                JobTitle =
                    connection.UserAId == userId
                        ? connection.UserB.JobTitle ?? string.Empty
                        : connection.UserA.JobTitle ?? string.Empty,
                ProfilePhotoId =
                    connection.UserAId == userId
                        ? connection.UserB.PhotographId
                        : connection.UserA.PhotographId,
            };
        query =
            query.
            OrderBy(sorting ?? DefaultSorting).
            PageBy(skipCount, maxResultCount);

        return await query.ToListAsync(cancellationToken);
    }

    public override async Task<IQueryable<Connection>> WithDetailsAsync(CancellationToken cancellationToken = default)
    {
        var queryable = (await GetQueryableAsync(cancellationToken))
            .Include(e => e.UserA)
            .Include(e => e.UserB);
        return queryable;
    }
}