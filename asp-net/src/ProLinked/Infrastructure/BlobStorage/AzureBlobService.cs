using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ProLinked.Shared.Exceptions;

namespace ProLinked.Infrastructure.BlobStorage;

public class AzureBlobService: IBlobService
{
    public string ContainerName { get; } = "pro-linked";

    private readonly BlobContainerClient _blobContainerClient;
    private BlobContainerInfo? _blobContainerInfo;

    public AzureBlobService(
        BlobContainerClient blobContainerClient)
    {
        _blobContainerClient = blobContainerClient;
    }



    public async Task<bool> ExistsAsync(string storageFileName, CancellationToken cancellationToken = default)
    {
        await GetOrCreateContainerIfNotExistsAsync(cancellationToken);
        var blobClient = _blobContainerClient.GetBlobClient(storageFileName);
        return await blobClient.ExistsAsync(cancellationToken);
    }

    public async Task SaveAsync(string storageFileName, byte[] byteArray,
        CancellationToken cancellationToken = default)
    {
        await GetOrCreateContainerIfNotExistsAsync(cancellationToken);
        var blobClient = _blobContainerClient.GetBlobClient(storageFileName);
        var ms = new MemoryStream(byteArray);
        await blobClient.UploadAsync(ms, true, cancellationToken);
    }

    public async Task<Stream?> FindAsync(string storageFileName, CancellationToken cancellationToken = default)
    {
        if (_blobContainerInfo is null)
        {
            _blobContainerInfo = (await _blobContainerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken)).Value;
            return null;
        }

        return await DownloadToStreamAsync(storageFileName, cancellationToken);
    }

    public async Task<Stream> GetAsync(string storageFileName, CancellationToken cancellationToken = default)
    {
        if (_blobContainerInfo is null)
        {
            _blobContainerInfo = (await _blobContainerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken)).Value;
            throw new BlobNotFoundException();
        }

        return await DownloadToStreamAsync(storageFileName, cancellationToken);
    }

    public async Task<bool> DeleteAsync(string storageFileName, CancellationToken cancellationToken = default)
    {
        var blobClient = _blobContainerClient.GetBlobClient(storageFileName);
        var result =
            await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, cancellationToken:cancellationToken);
        return result.HasValue && result.Value;
    }

    private async Task GetOrCreateContainerIfNotExistsAsync(CancellationToken cancellationToken)
    {
        _blobContainerInfo = (await _blobContainerClient.CreateIfNotExistsAsync(cancellationToken:cancellationToken)).Value;
    }

    private async Task<Stream> DownloadToStreamAsync(string storageFileName, CancellationToken cancellationToken)
    {
        var blobClient = _blobContainerClient.GetBlobClient(storageFileName);
        var ms = new MemoryStream();
        _ = await blobClient.DownloadToAsync(ms, cancellationToken);
        return ms;
    }
}
