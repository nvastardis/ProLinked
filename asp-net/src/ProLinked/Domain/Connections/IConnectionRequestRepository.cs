using ProLinked.Shared;
using ProLinked.Shared.Connections;

namespace ProLinked.Domain.Connections;

public interface IConnectionRequestRepository: IRepository<ConnectionRequest, Guid>
{
    Task<List<ConnectionRequestLookUp>> GetListByUserAsTargetAsync(
        Guid userId,
        ConnectionRequestStatus status = ConnectionRequestStatus.UNDEFINED,
        int skipCount = ProLinkedConsts.SkipCountDefaultValue,
        int maxResultCount = ProLinkedConsts.MaxResultCountDefaultValue,
        CancellationToken cancellationToken = default);
}
