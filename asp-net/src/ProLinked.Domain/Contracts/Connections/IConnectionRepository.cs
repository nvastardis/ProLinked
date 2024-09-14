using ProLinked.Domain.DTOs.Connections;
using ProLinked.Domain.Entities.Connections;

namespace ProLinked.Domain.Contracts.Connections;

public interface IConnectionRepository: IRepository<Connection, Guid>
{
    Task<List<ConnectionLookUp>> GetListByUserAsync(
        Guid userId,
        string? sorting = null,
        int skipCount = ProLinkedConsts.SkipCountDefaultValue,
        int maxResultCount = ProLinkedConsts.MaxResultCountDefaultValue,
        CancellationToken cancellationToken = default);
}