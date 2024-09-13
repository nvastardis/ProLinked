using ProLinked.Domain.DTOs.Blobs;
using ProLinked.Domain.Entities.Blobs;

namespace ProLinked.Application.Services.Blobs;

public interface IBlobManager
{
    Task<Blob> SaveAsync(
        Guid userId,
        string? fileName,
        byte[] byteArray,
        CancellationToken cancellationToken = default);

    Task<BlobWithData> GetAsync(
        Guid blobId,
        CancellationToken cancellationToken = default);

    Task<Stream> GetAllBytesAsync(
        Guid blobId,
        CancellationToken cancellationToken = default);

    Task<byte[]> GetAllBytesAsync(
        string storageFileName,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        string storageFileName,
        CancellationToken cancellationToken = default);
}
