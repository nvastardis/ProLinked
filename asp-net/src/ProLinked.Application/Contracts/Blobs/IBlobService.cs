using Microsoft.AspNetCore.Http;
using ProLinked.Application.DTOs.Blobs;

namespace ProLinked.Application.Contracts.Blobs;

public interface IBlobService
{
    Task<BlobDownloadDto> GetAsync(
        Guid input,
        CancellationToken cancellationToken = default);
    Task<BlobDownloadDto> GetManyAsync(
        Guid[] input,
        CancellationToken cancellationToken = default);
    Task PostAsync(
        IFormFile input,
        Guid userId,
        CancellationToken cancellationToken = default);
    Task PostManyAsync(
        IFormFileCollection input,
        Guid userId,
        CancellationToken cancellationToken = default);
    Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);
    Task DeleteManyAsync(
        Guid[] blobIds,
        CancellationToken cancellationToken = default);
}