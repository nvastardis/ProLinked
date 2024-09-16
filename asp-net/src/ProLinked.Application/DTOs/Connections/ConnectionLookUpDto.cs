namespace ProLinked.Application.DTOs.Connections;

public class ConnectionLookUpDto: EntityDto<Guid>
{
    public Guid UserId { get; set; }
    public string UserFullName { get; set; } = null!;
    public Guid? ProfilePhotoId { get; set; }
    public DateTime CreationTime { get; set; }
    public string? JobTitle { get; set; }
}