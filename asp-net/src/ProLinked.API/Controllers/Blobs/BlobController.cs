using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProLinked.Application.Contracts.Blobs;
using ProLinked.Application.Contracts.Blobs.DTOs;
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
    public async Task<Results<FileStreamHttpResult, BadRequest<string>, ProblemHttpResult>> GetAsync(
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
    public async Task<Results<FileStreamHttpResult, BadRequest<string>, ProblemHttpResult>> GetListAsync(
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
    public async Task<Results<Created, BadRequest<string>, ProblemHttpResult>> Upload(
        [Required] IFormFile input,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await CreatedWithStandardExceptionHandling(
            _blobService.PostAsync(
                input,
                userId,
                cancellationToken)
        );
    }

    [HttpPost]
    [Route("upload/list")]
    [Authorize]
    public async Task<Results<NoContent, BadRequest<string>, ProblemHttpResult>> PostManyAsync(
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
    public async Task<Results<NoContent, BadRequest<string>, ProblemHttpResult>> DeleteAsync(
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
    public async Task<Results<NoContent, BadRequest<string>, ProblemHttpResult>> DeleteListAsync(
        [FromQuery, Length(1,10)] Guid[] ids,
        CancellationToken cancellationToken = default)
    {
        return await NoContentWithStandardExceptionHandling(
            _blobService.DeleteManyAsync(
                ids,
                cancellationToken)
        );
    }

    private async Task<Results<FileStreamHttpResult, BadRequest<string>, ProblemHttpResult>> FileStreamWithStandardExceptionHandling(
        Task<BlobDownloadDto> taskToExecute)
    {
        try
        {
            var result = await taskToExecute;
            return TypedResults.File(result.Data, result.ContentType, result.FileName);
        }
        catch (BusinessException businessException)
        {
            Logger.LogError(businessException.Code);
            return TypedResults.BadRequest(businessException.Code);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex.Message);
            return TypedResults.Problem(ex.Message);
        }
    }
}