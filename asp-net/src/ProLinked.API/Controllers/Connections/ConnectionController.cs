using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProLinked.Application.Contracts.Connections;
using ProLinked.Application.DTOs;
using ProLinked.Application.DTOs.Connections;
using ProLinked.Application.DTOs.Filtering;
using ProLinked.Domain;
using System.ComponentModel.DataAnnotations;

namespace ProLinked.API.Controllers.Connections;

[ApiController]
[Route("api/connection")]
public class ConnectionController: ProLinkedController
{
    private readonly IConnectionService _connectionService;

    public ConnectionController(IConnectionService connectionService)
    {
        _connectionService = connectionService;
    }

    [HttpGet]
    [Route("list")]
    public async Task<Results<Ok<PagedAndSortedResultList<ConnectionLookUpDto>>,ProblemHttpResult>> GetListAsync(
        [Required, FromQuery] Guid userId,
        [FromQuery] string? sorting,
        [FromQuery] int? skipCount,
        [FromQuery] int? maxResultCount,
        CancellationToken cancellationToken = default) =>
        await OkWithStandardExceptionHandling(
            _connectionService.GetListAsync(
                new UserFilterDto
                {
                    UserId = userId,
                    IncludeDetails = false,
                    Sorting = sorting,
                    SkipCount = skipCount ?? ProLinkedConsts.SkipCountDefaultValue,
                    MaxResultCount = maxResultCount ?? ProLinkedConsts.MaxResultCountDefaultValue
                },
                cancellationToken)
            );

    [HttpDelete]
    [Route("delete/{id}")]
    public async Task<Results<NoContent, ProblemHttpResult>> GetListAsync(
        [Required] Guid id,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await NoContentWithStandardExceptionHandling(
            _connectionService.DeleteAsync(id, userId, cancellationToken));
    }
}