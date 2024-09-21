using AutoMapper;
using Microsoft.AspNetCore.Http;
using ProLinked.Application.Contracts.Blobs;
using ProLinked.Application.DTOs.Blobs;
using ProLinked.Domain.Contracts.Blobs;
using ProLinked.Domain.Extensions;
using System.IO.Compression;

namespace ProLinked.Application.Services.Blobs;

public class BlobService: ProLinkedServiceBase, IBlobService
{
    private IBlobManager BlobManager { get; }

    public BlobService(
        IMapper mapper,
        IBlobManager blobManager)
        : base(mapper)
    {
        BlobManager = blobManager;
    }

    public async Task<BlobDownloadDto> GetAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await DownloadFileAsync(id, cancellationToken);
    }

    public async Task<BlobDownloadDto> GetManyAsync(
        Guid[] input,
        CancellationToken cancellationToken = default)
    {
        var formFileList = new FormFileCollection();
        foreach (var item in input)
        {
            var newItem = await BlobManager.GetAsync(item, cancellationToken);
            var itemToAdd = new FormFile(
                newItem.Data,
                0,
                newItem.Data.Length,
                newItem.Info.FullFileName,
                newItem.Info.FullFileName);

            formFileList.Add(itemToAdd);
        }
        return await ZipFilesAsync(formFileList, cancellationToken);
    }

    public async Task PostAsync(
        IFormFile input,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var fileName = input.FileName;
        var data = await input.OpenReadStream().GetAllBytesAsync(cancellationToken);
        await UploadFileAsync(userId, fileName, data, cancellationToken);
    }

    public async Task PostManyAsync(
        IFormFileCollection input,
        Guid userId,
        CancellationToken cancellationToken = default)
    {

        foreach (var item in input)
        {
            var fileName = item.FileName;
            var data = await item.OpenReadStream().GetAllBytesAsync(cancellationToken);
            await UploadFileAsync(userId, fileName, data, cancellationToken);
        }

    }

    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var blobWithData = await BlobManager.GetAsync(id, cancellationToken: cancellationToken);
        await BlobManager.DeleteAsync(blobWithData.Info, cancellationToken);
    }

    public async Task DeleteManyAsync(
        Guid[] input,
        CancellationToken cancellationToken = default)
    {
        foreach (var id in input)
        {
            var blobWithData = await BlobManager.GetAsync(id, cancellationToken);
            await BlobManager.DeleteAsync(blobWithData.Info, cancellationToken);
        }
    }

    private async Task UploadFileAsync(
        Guid userId,
        string? fileName,
        byte[] data,
        CancellationToken cancellationToken = default)
    {
        await BlobManager.SaveAsync(
            userId,
            fileName,
            data,
            cancellationToken: cancellationToken
        );
    }

    private async Task<BlobDownloadDto> DownloadFileAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var result = await BlobManager.GetAsync(id, cancellationToken);
        if (result.Data.Length == 0)
        {
            throw new EndOfStreamException();
        }

        result.Data.Position = 0;
        return new BlobDownloadDto
        {
            FileName = result.Info.FullFileName,
            Data = result.Data
        };
    }

    private async Task<BlobDownloadDto> ZipFilesAsync(
        IFormFileCollection files,
        CancellationToken cancellationToken = default)
    {
        var zipName = $"Requested_Items_{DateTime.Now:yyyy_MM_dd-HH_mm_ss}.";
        var memoryStream = new MemoryStream();
        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        {
            foreach (var file in files)
            {
                var fileBytes = await file.OpenReadStream().GetAllBytesAsync(cancellationToken);
                var demoFile = archive.CreateEntry(file.FileName ?? throw new NullReferenceException());
                using var ms = new MemoryStream(fileBytes);
                {
                    await using var entryStream = demoFile.Open();
                    await ms.CopyToAsync(entryStream, cancellationToken);
                }
            }
        }


        memoryStream.Position = 0;
        return new BlobDownloadDto
        {
            Data = memoryStream,
            FileName = zipName,
            ContentType = "application/zip"
        };
    }
}