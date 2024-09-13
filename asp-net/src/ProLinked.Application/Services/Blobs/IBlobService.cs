namespace ProLinked.Application.Services.Blobs;

public interface IBlobService
{
    string ContainerName { get; }

    Task<bool> ExistsAsync(
        string storageFileName,
        CancellationToken cancellationToken = default);

    Task SaveAsync(
        string storageFileName,
        byte[] byteArray,
        CancellationToken cancellationToken = default);

    Task<Stream?> FindAsync(
        string storageFileName,
        CancellationToken cancellationToken = default);

    Task<Stream> GetAsync(
        string storageFileName,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        string storageFileName,
        CancellationToken cancellationToken = default);
}
