using ProLinked.Domain.AzureStorage.Blobs;
using ProLinked.Infrastructure.BlobStorage;
using ProLinked.Shared;
using ProLinked.Shared.AzureStorage;
using ProLinked.Shared.Exceptions;
using ProLinked.Shared.Extensions;
using ProLinked.Shared.Utils;

namespace ProLinked.Domain.Blobs;

public class BlobManager: IDomainService
{
    private readonly IBlobService _blobService;
    private readonly IRepository<Blob, Guid> _blobRepository;
    private readonly Dictionary<string, BlobTypeEnum> _extensionToTypeMap;

    public BlobManager(
        IBlobService blobService,
        IRepository<Blob, Guid> blobRepository)
    {
        _blobService = blobService;
        _blobRepository = blobRepository;
        _extensionToTypeMap = new Dictionary<string, BlobTypeEnum>()
        {
            { ".pdf", BlobTypeEnum.Document },
            { ".jpeg", BlobTypeEnum.Picture },
            { ".png", BlobTypeEnum.Picture },
            { ".gif", BlobTypeEnum.Gif },
            { ".mp4", BlobTypeEnum.Video },
            { ".mov", BlobTypeEnum.Video },
            { ".flv", BlobTypeEnum.Video }
        };
    }

    public async Task<Blob> SaveAsync(
        Guid userId,
        string? fileName,
        byte[] byteArray,
        CancellationToken cancellationToken = default)
    {
        if (fileName.IsNullOrWhiteSpace())
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.InvalidBlobFileName);
        }
        var blobId = Guid.NewGuid();
        var extension = Path.GetExtension(fileName) ?? string.Empty;
        var storageFileName = $"{Path.GetFileNameWithoutExtension(fileName)}_{blobId}{extension}";
        if( !_extensionToTypeMap.TryGetValue(extension, out var blobType))
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.InvalidBlobType)
                .WithData(nameof(Blob.Extension), extension);
        }

        await _blobService.SaveAsync(storageFileName, byteArray, cancellationToken);
        var blob = new Blob(
            blobId,
            userId,
            blobType,
            fileName!,
            extension,
            storageFileName);
        return blob;
    }

    public async Task<BlobWithData> GetAsync(
        Guid blobId,
        CancellationToken cancellationToken = default)
    {
        var blob = await _blobRepository.FindAsync(blobId, cancellationToken:cancellationToken);
        if (blob is null)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.BlobNotFound)
                .WithData(nameof(Blob.Id), blobId);
        }

        if ( !await _blobService.ExistsAsync(blob.StorageFileName, cancellationToken))
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.BlobNotFound)
                .WithData(nameof(Blob.StorageFileName), blob.StorageFileName)
                .WithData("StorageName", nameof(AzureBlobService.ContainerName));
        }

        var data = await _blobService.GetAsync(blob.StorageFileName, cancellationToken);

        return new BlobWithData(blob, data);
    }

    public async Task<Stream> GetAllBytesAsync(
        Guid blobId,
        CancellationToken cancellationToken = default)
    {
        var blob = await _blobRepository.FindAsync(blobId, cancellationToken:cancellationToken);
        if (blob is null)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.BlobNotFound)
                .WithData("BlobId", blobId);
        }

        if ( !await _blobService.ExistsAsync(blob.StorageFileName, cancellationToken))
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.BlobNotFound)
                .WithData("StorageFileName", blob.StorageFileName)
                .WithData("StorageName", nameof(AzureBlobService.ContainerName));
        }
        return await _blobService.GetAsync(blob.StorageFileName, cancellationToken);
    }

    public async Task<byte[]> GetAllBytesAsync(string storageFileName, CancellationToken cancellationToken)
    {
        if ( !await _blobService.ExistsAsync(storageFileName, cancellationToken))
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.BlobNotFound)
                .WithData("StorageFileName", storageFileName)
                .WithData("StorageName", nameof(AzureBlobService.ContainerName));
        }
        var result =  await _blobService.GetAsync(storageFileName, cancellationToken);
        return await result.GetAllBytesAsync(cancellationToken);

    }

    public async Task DeleteAsync(string storageFileName, CancellationToken cancellationToken)
    {
        await _blobService.DeleteAsync(storageFileName, cancellationToken);
    }
}
