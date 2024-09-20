using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ProLinked.Application.Contracts.Blobs;

public interface IBlobService
{
    Task<Results<FileStreamHttpResult, ProblemHttpResult>> GetAsync(
        Guid input,
        CancellationToken cancellationToken = default);
    Task<Results<FileStreamHttpResult, ProblemHttpResult>> GetManyAsync(
        Guid[] input,
        CancellationToken cancellationToken = default);
    Task<Results<NoContent, ProblemHttpResult>> PostAsync(
        IFormFile input,
        Guid userId,
        CancellationToken cancellationToken = default);
    Task<Results<NoContent, ProblemHttpResult>> PostManyAsync(
        IFormFileCollection input,
        Guid userId,
        CancellationToken cancellationToken = default);
    Task<Results<NoContent, ProblemHttpResult>> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);
    Task<Results<NoContent, ProblemHttpResult>> DeleteManyAsync(
        Guid[] blobIds,
        CancellationToken cancellationToken = default);
}