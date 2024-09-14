using Microsoft.EntityFrameworkCore;
using ProLinked.Domain;
using ProLinked.Domain.Contracts.Connections;
using ProLinked.Domain.DTOs.Connections;
using ProLinked.Domain.Entities.Connections;
using ProLinked.Domain.Extensions;
using ProLinked.Domain.Shared.Connections;
using ProLinked.Infrastructure.Data;

namespace ProLinked.Infrastructure.Data.Repositories.Connections;

public class ConnectionRequestRepository: ProLinkedBaseRepository<ConnectionRequest, Guid>, IConnectionRequestRepository
{
    public ConnectionRequestRepository(
        ProLinkedDbContext dbContext) :
        base(dbContext)
    {
        DefaultSorting = $"{nameof(ConnectionRequest.CreationTime)} DESC";
    }

    public async Task<List<ConnectionRequestLookUp>> GetListByUserAsTargetAsync(
        Guid userId,
        ConnectionRequestStatus status = ConnectionRequestStatus.UNDEFINED,
        int skipCount = ProLinkedConsts.SkipCountDefaultValue,
        int maxResultCount = ProLinkedConsts.MaxResultCountDefaultValue,
        CancellationToken cancellationToken = default)
    {
        var requestQueryable = await WithDetailsAsync(cancellationToken);
        var query =
            from request in requestQueryable
            where (request.TargetId == userId && request.Status == status)
            select new ConnectionRequestLookUp()
            {
                Id = request.Id,
                UserId = request.SenderId,
                UserFullName = $"{request.Sender.Name} {request.Sender.Surname}",
                CreationTime = request.CreationTime,
                JobTitle = request.Sender.JobTitle ?? string.Empty,
                PhotoId = request.Sender.PhotographId ?? Guid.Empty
            };
        query =
            query.
            OrderByDescending(e => e.CreationTime).
            PageBy(skipCount, maxResultCount);

        return await query.ToListAsync(cancellationToken);
    }

    public override async Task<IQueryable<ConnectionRequest>> WithDetailsAsync(CancellationToken cancellationToken = default)
    {
        return (await GetQueryableAsync(cancellationToken)).
            Include(x => x.Sender).
            Include(x => x.Target);
    }
}