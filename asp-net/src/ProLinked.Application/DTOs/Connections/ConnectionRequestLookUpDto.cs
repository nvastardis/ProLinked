namespace ProLinked.Application.DTOs.Connections;

public class ConnectionRequestLookUpDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserFullName { get; set; } = null!;
    public DateTime CreationTime { get; set; }
    public Guid? ProfilePhotoId { get; set; }
    public string? JobTitle { get; set; }
}