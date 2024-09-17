using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using ProLinked.Application.Contracts.Blobs;
using ProLinked.Domain;
using ProLinked.Domain.Contracts.Blobs;
using ProLinked.Domain.Extensions;
using System.ComponentModel.DataAnnotations;
using System.IO.Compression;
using PLBlob = ProLinked.Domain.Entities.Blobs.Blob;

namespace ProLinked.Application.Services.Blobs;

public class BlobService: ProLinkedServiceBase, IBlobService
{
    private IBlobManager BlobManager { get; }
    private IRepository<PLBlob,Guid> BlobRepository { get; }

    public BlobService(
        IMapper mapper,
        ILogger<IBlobService> logger,
        IBlobManager blobManager,
        IRepository<PLBlob, Guid> blobRepository)
        : base(mapper, logger)
    {
        BlobManager = blobManager;
        BlobRepository = blobRepository;
    }

    public async Task<Results<FileStreamHttpResult, ProblemHttpResult>> GetAsync(
        [Required] Guid id,
        CancellationToken cancellationToken = default)
    {
        return await DownloadFileAsync(id, cancellationToken);
    }

    public async Task<Results<FileStreamHttpResult, ProblemHttpResult>> GetManyAsync(
        [Length(1,10)] Guid[] input,
        CancellationToken cancellationToken = default)
    {
        try
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
        catch (Exception ex)
        {
            Logger.LogDebug(ex.Message);
            return TypedResults.Problem();
        }
    }

    public async Task<Results<NoContent, ProblemHttpResult>> PostAsync(
        [Required] IFormFile input,
        [Required] Guid userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var fileName = input.FileName;
            var data = await input.OpenReadStream().GetAllBytesAsync(cancellationToken);
            await UploadFileAsync(userId, fileName, data, cancellationToken);
            return TypedResults.NoContent();
        }
        catch (Exception ex)
        {
            Logger.LogDebug(ex.Message);
            return TypedResults.Problem();
        }
    }

    public async Task<Results<NoContent, ProblemHttpResult>> PostManyAsync(
        [Required] IFormFileCollection input,
        [Required] Guid userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            foreach (var item in input)
            {
                var fileName = item.FileName;
                var data = await item.OpenReadStream().GetAllBytesAsync(cancellationToken);
                await UploadFileAsync(userId, fileName, data, cancellationToken);
            }
            return TypedResults.NoContent();
        }
        catch (Exception ex)
        {
            Logger.LogDebug(ex.Message);
            return TypedResults.Problem();
        }
    }

    public async Task<Results<NoContent, ProblemHttpResult>> DeleteAsync(
        [Required] Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var blob = await BlobRepository.GetAsync(id, cancellationToken: cancellationToken);
            await BlobManager.DeleteAsync(blob.StorageFileName, cancellationToken);
            await BlobRepository.DeleteAsync(blob, autoSave: true, cancellationToken);
            return TypedResults.NoContent();
        }
        catch (Exception ex)
        {
            Logger.LogDebug(ex.Message);
            return TypedResults.Problem();
        }
    }

    public async Task<Results<NoContent, ProblemHttpResult>> DeleteManyAsync(
        [Length(1,10)] Guid[] input,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var blobs = await BlobRepository.FindManyAsync(e => input.Contains(e.Id),
                cancellationToken: cancellationToken);
            if (blobs is null)
            {
                return TypedResults.Problem();
            }

            foreach (var blob in blobs)
            {
                await BlobManager.DeleteAsync(blob.StorageFileName, cancellationToken);
            }

            await BlobRepository.DeleteManyAsync(input, autoSave: true, cancellationToken: cancellationToken);
            return TypedResults.NoContent();
        }
        catch (Exception ex)
        {
            Logger.LogDebug(ex.Message);
            return TypedResults.Problem();
        }
    }

    private async Task UploadFileAsync(Guid userId, string? fileName, byte[] data, CancellationToken cancellationToken = default)
    {
        var newBlob = await BlobManager.SaveAsync(
            userId,
            fileName,
            data,
            cancellationToken: cancellationToken
        );
        await BlobRepository.InsertAsync(newBlob, autoSave:true, cancellationToken: cancellationToken);
    }

    private async Task<Results<FileStreamHttpResult, ProblemHttpResult>> DownloadFileAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await BlobManager.GetAsync(id, cancellationToken);
        if (result.Data.Length == 0)
        {
            return TypedResults.Problem();
        }

        result.Data.Position = 0;
        return TypedResults.File(fileStream:result.Data, fileDownloadName:result.Info.FullFileName);
    }

    private async Task<FileStreamHttpResult> ZipFilesAsync(
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
        return TypedResults.File(memoryStream, "application/zip", zipName);
    }
}