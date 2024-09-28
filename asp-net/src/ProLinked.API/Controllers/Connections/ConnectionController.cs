using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProLinked.Application.Contracts.Connections;
using ProLinked.Application.Contracts.Connections.DTOs;
using ProLinked.Application.Contracts.Filtering;
using ProLinked.Application.DTOs;
using ProLinked.Domain;
using System.ComponentModel.DataAnnotations;

namespace ProLinked.API.Controllers.Connections;

[ApiController]
[Route("api/connection")]
public class ConnectionController: ProLinkedController
{
    private readonly IConnectionService _connectionService;

    public ConnectionController(
        ILogger<ConnectionController> logger,
        IConnectionService connectionService)
        : base(logger)
    {
        _connectionService = connectionService;
    }

    [HttpGet]
    [Route("list")]
    public async Task<Results<Ok<PagedAndSortedResultList<ConnectionLookUpDto>>, BadRequest<string>, ProblemHttpResult>> GetListAsync(
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
    public async Task<Results<NoContent, BadRequest<string>, ProblemHttpResult>> GetListAsync(
        [Required] Guid id,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await NoContentWithStandardExceptionHandling(
            _connectionService.DeleteAsync(id, userId, cancellationToken));
    }
}