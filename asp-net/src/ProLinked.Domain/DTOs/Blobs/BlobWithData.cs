using ProLinked.Domain.Entities.Blobs;

namespace ProLinked.Domain.DTOs.Blobs;

public class BlobWithData
{
    public Blob Info { get; set; }
    public Stream Data { get; set; }

    public BlobWithData(
        Blob blob,
        Stream data)
    {
        Info = blob;
        Data = data;
    }
}
