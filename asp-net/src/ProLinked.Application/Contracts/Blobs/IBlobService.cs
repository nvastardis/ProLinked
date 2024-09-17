using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel.DataAnnotations;

namespace ProLinked.Application.Contracts.Blobs;

public interface IBlobService
{
    Task<Results<FileStreamHttpResult, ProblemHttpResult>> GetAsync(
        [Required] Guid input,
        CancellationToken cancellationToken = default);
    Task<Results<FileStreamHttpResult, ProblemHttpResult>> GetManyAsync(
        [Length(1,10)] Guid[] input,
        CancellationToken cancellationToken = default);
    Task<Results<NoContent, ProblemHttpResult>> PostAsync(
        [Required] IFormFile input,
        [Required] Guid userId,
        CancellationToken cancellationToken = default);
    Task<Results<NoContent, ProblemHttpResult>> PostManyAsync(
        [Length(1,10)] IFormFileCollection input,
        [Required] Guid userId,
        CancellationToken cancellationToken = default);
    Task<Results<NoContent, ProblemHttpResult>> DeleteAsync(
        [Required] Guid id,
        CancellationToken cancellationToken = default);
    Task<Results<NoContent, ProblemHttpResult>> DeleteManyAsync(
        [Length(1,10)] Guid[] blobIds,
        CancellationToken cancellationToken = default);
}