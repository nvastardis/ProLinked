using ProLinked.Shared;

namespace ProLinked.Domain.Connections;

public interface IConnectionRepository: IRepository<Connection, Guid>
{
    Task<List<ConnectionLookUp>> GetListByUserAsync(
        Guid userId,
        string? sorting = null,
        int skipCount = ProLinkedConsts.SkipCountDefaultValue,
        int maxResultCount = ProLinkedConsts.MaxResultCountDefaultValue,
        CancellationToken cancellationToken = default);
}
