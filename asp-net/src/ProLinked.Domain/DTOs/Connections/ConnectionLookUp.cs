namespace ProLinked.Domain.DTOs.Connections;

public class ConnectionLookUp
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid? ProfilePhotoId { get; set; }
    public string UserFullName { get; set; } = null!;
    public DateTime CreationTime { get; set; }
    public string? JobTitle { get; set; }
}