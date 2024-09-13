using ProLinked.Domain.Shared.Blobs;

namespace ProLinked.Domain.Entities.Blobs;

public class Blob: Entity<Guid>
{
    public BlobTypeEnum Type { get; init; }
    public string FullFileName{ get; init; }
    public string Extension { get; init; }
    public string StorageFileName { get; init; }
    public Guid CreatorId { get; init; }
    public DateTime CreationTime { get; init; }

    public Blob(
        Guid id,
        Guid creatorId,
        BlobTypeEnum type,
        string fullFileName,
        string extension,
        string storageFileName)
        : base(id)
    {
        CreatorId = creatorId;
        Type = type;
        FullFileName = fullFileName;
        Extension = extension;
        StorageFileName = storageFileName;
        CreationTime = DateTime.Now;
    }
}
