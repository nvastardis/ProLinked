using ProLinked.Domain;
using ProLinked.Domain.DTOs.Connections;
using ProLinked.Domain.Entities.Connections;
using ProLinked.Domain.Shared;

namespace ProLinked.Application.Services.Connections;

public interface IConnectionRepository: IRepository<Connection, Guid>
{
    Task<List<ConnectionLookUp>> GetListByUserAsync(
        Guid userId,
        string? sorting = null,
        int skipCount = ProLinkedConsts.SkipCountDefaultValue,
        int maxResultCount = ProLinkedConsts.MaxResultCountDefaultValue,
        CancellationToken cancellationToken = default);
}
