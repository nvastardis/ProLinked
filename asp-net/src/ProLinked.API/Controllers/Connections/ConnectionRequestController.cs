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
[Route("api/connection-request")]
public class ConnectionRequestController: ProLinkedController
{
    private readonly IConnectionRequestService _connectionRequestService;

    public ConnectionRequestController(
        IConnectionRequestService connectionRequestService)
    {
        _connectionRequestService = connectionRequestService;
    }

    [HttpGet]
    [Route("pending")]
    public async Task<Results<Ok<PagedAndSortedResultList<ConnectionRequestLookUpDto>>, ProblemHttpResult>>
        GetListPendingAsync(
            [FromQuery] string? sorting,
            [FromQuery] int? skipCount,
            [FromQuery] int? maxResultCount,
            CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await OkWithStandardExceptionHandling(
            _connectionRequestService.GetListPendingAsync(
                new ListFilterDto
                {
                    IncludeDetails = false,
                    Sorting = sorting,
                    SkipCount = skipCount ?? ProLinkedConsts.SkipCountDefaultValue,
                    MaxResultCount = maxResultCount ?? ProLinkedConsts.MaxResultCountDefaultValue
                },
                userId,
                cancellationToken)
        );
    }

    [HttpGet]
    [Route("find-pending-for-user/{targertId}")]
    public async Task<Results<Ok<ConnectionRequestSearchResultDto>, ProblemHttpResult>> FindPendingForUserAsync(
        [Required] Guid targertId,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await OkWithStandardExceptionHandling(
            _connectionRequestService.FindPendingForUserAsync(
                userId,
                targertId,
                cancellationToken)
        );
    }

    [HttpPost]
    public async Task<Results<NoContent, ProblemHttpResult>> CreateAsync(
        ConnectionRequestCreateDto input,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await NoContentWithStandardExceptionHandling(
            _connectionRequestService.CreateAsync(
                input,
                userId,
                cancellationToken)
        );
    }

    [HttpPut]
    [Route("accept/{id}")]
    public async Task<Results<NoContent, ProblemHttpResult>> AcceptAsync(
        [Required] Guid id,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await NoContentWithStandardExceptionHandling(
            _connectionRequestService.AcceptAsync(
                id,
                userId,
                cancellationToken)
        );
    }

    [HttpPut]
    [Route("reject/{id}")]
    public async Task<Results<NoContent, ProblemHttpResult>> RejectAsync(
        [Required] Guid id,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await NoContentWithStandardExceptionHandling(
            _connectionRequestService.RejectAsync(
                id,
                userId,
                cancellationToken)
        );
    }

    [HttpDelete]
    [Route("delete/{id}")]
    public async Task<Results<NoContent, ProblemHttpResult>> DeleteAsync(
        [Required] Guid id,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await NoContentWithStandardExceptionHandling(
            _connectionRequestService.DeleteAsync(
                id,
                userId,
                cancellationToken)
        );
    }
}