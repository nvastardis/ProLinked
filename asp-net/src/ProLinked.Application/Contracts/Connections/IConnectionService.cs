using ProLinked.Application.DTOs;
using ProLinked.Application.DTOs.Connections;
using ProLinked.Application.DTOs.Filtering;
using System.ComponentModel.DataAnnotations;

namespace ProLinked.Application.Contracts.Connections;

public interface IConnectionService
{
    Task<PagedAndSortedResultList<ConnectionLookUpDto>> GetListAsync(
        [Required] UserFilterDto input,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        [Required] Guid id,
        [Required] Guid userId,
        CancellationToken cancellationToken = default);
}