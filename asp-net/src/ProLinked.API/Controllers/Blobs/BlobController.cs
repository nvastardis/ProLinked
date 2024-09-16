using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ProLinked.Application.Contracts.Blobs;
using System.ComponentModel.DataAnnotations;

namespace ProLinked.API.Controllers.Blobs;

[ApiController]
[Route("api/blob")]
public class BlobController: Controller
{
    private readonly IBlobService _blobService;

    public BlobController(IBlobService blobService)
    {
        _blobService = blobService;
    }

    [HttpGet]
    [Authorize]
    [Route("{id}")]
    public async Task<Results<FileStreamHttpResult, ProblemHttpResult>> GetAsync(
        [Required] Guid id,
        CancellationToken cancellationToken = default) =>
        await _blobService.GetAsync(
            id,
            cancellationToken);

    [HttpGet]
    [Authorize]
    [Route("list")]
    public async Task<Results<FileStreamHttpResult, ProblemHttpResult>> GetListAsync(
        [FromBody] [Length(1,10)] Guid[] ids,
        CancellationToken cancellationToken = default) =>
        await _blobService.GetManyAsync(
            ids,
            cancellationToken);

    [HttpPost]
    [Route("upload")]
    [Authorize]
    public async Task Upload(
        [FromBody] [Required] IFormFile input,
        CancellationToken cancellationToken = default)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty);
        await _blobService.PostAsync(input, userId, cancellationToken);
    }

    [HttpPost]
    [Route("upload/list")]
    [Authorize]
    public async Task PostManyAsync(
        [FromBody] [Length(1, 10)] IFormFileCollection input,
        CancellationToken cancellationToken = default)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty);
        await _blobService.PostManyAsync(input, userId, cancellationToken);
    }

    [HttpDelete]
    [Route("delete/{id}")]
    [Authorize]
    public async Task DeleteAsync(
        [Required] Guid id,
        CancellationToken cancellationToken = default) =>
        await _blobService.DeleteAsync(id, cancellationToken);

    [HttpDelete]
    [Route("delete/list")]
    [Authorize]
    public async Task DeleteListAsync(
        [FromBody] [Length(1,10)] Guid[] ids,
        CancellationToken cancellationToken = default) =>
        await _blobService.DeleteManyAsync(ids, cancellationToken);
}