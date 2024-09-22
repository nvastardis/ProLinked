namespace ProLinked.Application.Contracts.Blobs.DTOs;

public class BlobDownloadDto
{
    public string FileName = null!;
    public Stream Data = null!;
    public string? ContentType= null;
}