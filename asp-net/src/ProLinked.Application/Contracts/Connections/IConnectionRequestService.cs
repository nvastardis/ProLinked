using ProLinked.Application.DTOs;
using ProLinked.Application.DTOs.Connections;
using ProLinked.Application.DTOs.Filtering;
using System.ComponentModel.DataAnnotations;

namespace ProLinked.Application.Contracts.Connections;

public interface IConnectionRequestService
{
    Task<PagedAndSortedResultList<ConnectionRequestLookUpDto>> GetListPendingAsync(
        [Required] ListFilterDto input,
        [Required] Guid userId,
        CancellationToken cancellationToken = default);

    Task<ConnectionRequestSearchResultDto> FindPendingForUserAsync(
        [Required] Guid currentUserId,
        [Required] Guid targetUserId,
        CancellationToken cancellationToken = default);

    Task CreateAsync(
        [Required] ConnectionRequestCreateDto input,
        [Required] Guid userId,
        CancellationToken cancellationToken = default);

    Task AcceptAsync(
        [Required] Guid id,
        [Required] Guid userId,
        CancellationToken cancellationToken = default);

    Task RejectAsync(
        [Required] Guid id,
        [Required] Guid userId,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        [Required] Guid id,
        [Required] Guid userId,
        CancellationToken cancellationToken = default);
}