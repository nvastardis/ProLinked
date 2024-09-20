using ProLinked.Application.DTOs;
using ProLinked.Application.DTOs.Connections;
using ProLinked.Application.DTOs.Filtering;

namespace ProLinked.Application.Contracts.Connections;

public interface IConnectionRequestService
{
    Task<PagedAndSortedResultList<ConnectionRequestLookUpDto>> GetListPendingAsync(
        ListFilterDto input,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<ConnectionRequestSearchResultDto> FindPendingForUserAsync(
        Guid currentUserId,
        Guid targetUserId,
        CancellationToken cancellationToken = default);

    Task CreateAsync(
        ConnectionRequestCreateDto input,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task AcceptAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task RejectAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default);
}