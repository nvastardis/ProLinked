namespace ProLinked.Domain.Entities.Posts;

public class PostBlob: Entity
{
    public Guid PostId { get; init; }
    public Guid BlobId { get; init; }

    private PostBlob(){ }

    public PostBlob(
        Guid postId,
        Guid blobId)
    {
        PostId = postId;
        BlobId = blobId;
    }

    public override object?[] GetKeys()
    {
        return [PostId, BlobId];
    }
}
