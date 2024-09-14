using ProLinked.Domain.DTOs.Connections;
using ProLinked.Domain.Entities.Connections;
using ProLinked.Domain.Shared.Connections;

namespace ProLinked.Domain.Contracts.Connections;

public interface IConnectionRequestRepository: IRepository<ConnectionRequest, Guid>
{
    Task<List<ConnectionRequestLookUp>> GetListByUserAsTargetAsync(
        Guid userId,
        ConnectionRequestStatus status = ConnectionRequestStatus.UNDEFINED,
        int skipCount = ProLinkedConsts.SkipCountDefaultValue,
        int maxResultCount = ProLinkedConsts.MaxResultCountDefaultValue,
        CancellationToken cancellationToken = default);
}