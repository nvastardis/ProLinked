using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProLinked.Application.Contracts.Blobs;
using ProLinked.Application.DTOs.Blobs;
using ProLinked.Domain.Shared.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace ProLinked.API.Controllers.Blobs;

[ApiController]
[Route("api/blob")]
public class BlobController: ProLinkedController
{
    private readonly IBlobService _blobService;

    public BlobController(
        ILogger<BlobController> logger,
        IBlobService blobService)
        : base(logger)
    {
        _blobService = blobService;
    }

    [HttpGet]
    [Authorize]
    [Route("{id}")]
    public async Task<Results<FileStreamHttpResult, ProblemHttpResult>> GetAsync(
        [Required] Guid id,
        CancellationToken cancellationToken = default)
    {
        return await FileStreamWithStandardExceptionHandling(
            _blobService.GetAsync(
                id,
                cancellationToken)
        );
    }


    [HttpGet]
    [Authorize]
    [Route("list/")]
    public async Task<Results<FileStreamHttpResult, ProblemHttpResult>> GetListAsync(
        [FromQuery, Length(1, 10)] Guid[] ids,
        CancellationToken cancellationToken = default)
    {
        return await FileStreamWithStandardExceptionHandling(
            _blobService.GetManyAsync(
                ids,
                cancellationToken)
        );
    }

    [HttpPost]
    [Route("upload")]
    [Authorize]
    public async Task<Results<NoContent, ProblemHttpResult>> Upload(
        [Required] IFormFile input,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await NoContentWithStandardExceptionHandling(
            _blobService.PostAsync(
                input,
                userId,
                cancellationToken)
        );
    }

    [HttpPost]
    [Route("upload/list")]
    [Authorize]
    public async Task<Results<NoContent, ProblemHttpResult>> PostManyAsync(
        [Required, Length(1, 10)] IFormFileCollection input,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await NoContentWithStandardExceptionHandling(
            _blobService.PostManyAsync(
                input,
                userId,
                cancellationToken)
        );
    }

    [HttpDelete]
    [Route("delete/{id}")]
    [Authorize]
    public async Task<Results<NoContent, ProblemHttpResult>> DeleteAsync(
        [Required] Guid id,
        CancellationToken cancellationToken = default)
    {
        return await NoContentWithStandardExceptionHandling(
            _blobService.DeleteAsync(
                id,
                cancellationToken)
        );
    }

    [HttpDelete]
    [Route("delete/list")]
    [Authorize]
    public async Task<Results<NoContent, ProblemHttpResult>> DeleteListAsync(
        [FromQuery, Length(1,10)] Guid[] ids,
        CancellationToken cancellationToken = default)
    {
        return await NoContentWithStandardExceptionHandling(
            _blobService.DeleteManyAsync(
                ids,
                cancellationToken)
        );
    }

    private async Task<Results<FileStreamHttpResult, ProblemHttpResult>> FileStreamWithStandardExceptionHandling(
        Task<BlobDownloadDto> taskToExecute)
    {
        try
        {
            var result = await taskToExecute;
            return TypedResults.File(result.Data, result.ContentType, result.FileName);
        }
        catch (BusinessException businessException)
        {
            return TypedResults.Problem(businessException.Code);
        }
        catch (Exception ex)
        {
            return TypedResults.Problem(ex.Message);
        }
    }
}