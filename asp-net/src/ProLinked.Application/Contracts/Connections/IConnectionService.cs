using ProLinked.Application.DTOs;
using ProLinked.Application.DTOs.Connections;
using ProLinked.Application.DTOs.Filtering;

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