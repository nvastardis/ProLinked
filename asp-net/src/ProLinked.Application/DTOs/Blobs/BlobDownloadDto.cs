namespace ProLinked.Application.DTOs.Blobs;

public class BlobDownloadDto
{
    public string FileName = null!;
    public Stream Data = null!;
    public string? ContentType= null;
}