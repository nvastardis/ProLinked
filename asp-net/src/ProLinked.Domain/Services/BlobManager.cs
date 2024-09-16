using ProLinked.Domain.Azure;
using ProLinked.Domain.Contracts.Blobs;
using ProLinked.Domain.DTOs.Blobs;
using ProLinked.Domain.Entities.Blobs;
using ProLinked.Domain.Extensions;
using ProLinked.Domain.Shared;
using ProLinked.Domain.Shared.Blobs;
using ProLinked.Domain.Shared.Exceptions;

namespace ProLinked.Domain.Services;

public class BlobManager: IBlobManager
{
    private readonly IAzureBlobService _azureBlobService;
    private readonly IRepository<Blob, Guid> _blobRepository;
    private readonly Dictionary<string, BlobTypeEnum> _extensionToTypeMap;

    public BlobManager(
        IAzureBlobService azureBlobService,
        IRepository<Blob, Guid> blobRepository)
    {
        _azureBlobService = azureBlobService;
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

        await _azureBlobService.SaveAsync(storageFileName, byteArray, cancellationToken);
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

        if ( !await _azureBlobService.ExistsAsync(blob.StorageFileName, cancellationToken))
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.BlobNotFound)
                .WithData(nameof(Blob.StorageFileName), blob.StorageFileName)
                .WithData("StorageName", nameof(IAzureBlobService.ContainerName));
        }

        var data = await _azureBlobService.GetAsync(blob.StorageFileName, cancellationToken);

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

        if ( !await _azureBlobService.ExistsAsync(blob.StorageFileName, cancellationToken))
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.BlobNotFound)
                .WithData("StorageFileName", blob.StorageFileName)
                .WithData("StorageName", nameof(IAzureBlobService.ContainerName));
        }
        return await _azureBlobService.GetAsync(blob.StorageFileName, cancellationToken);
    }

    public async Task<byte[]> GetAllBytesAsync(
        string storageFileName,
        CancellationToken cancellationToken = default)
    {
        if ( !await _azureBlobService.ExistsAsync(storageFileName, cancellationToken))
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.BlobNotFound)
                .WithData("StorageFileName", storageFileName)
                .WithData("StorageName", nameof(IAzureBlobService.ContainerName));
        }
        var result =  await _azureBlobService.GetAsync(storageFileName, cancellationToken);
        return await result.GetAllBytesAsync(cancellationToken);

    }

    public async Task DeleteAsync(
        string storageFileName,
        CancellationToken cancellationToken = default)
    {
        await _azureBlobService.DeleteAsync(storageFileName, cancellationToken);
    }
}