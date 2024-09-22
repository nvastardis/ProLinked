using ProLinked.Application.Contracts.Connections.DTOs;
using ProLinked.Application.Contracts.Filtering;
using ProLinked.Application.DTOs;

namespace ProLinked.Application.Contracts.Connections;

public interface IConnectionService
{
    Task<PagedAndSortedResultList<ConnectionLookUpDto>> GetListAsync(
        UserFilterDto input,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default);
}