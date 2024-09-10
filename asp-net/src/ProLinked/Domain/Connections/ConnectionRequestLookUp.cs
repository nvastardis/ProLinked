using System;
namespace ProLinked.Domain.Connections;

public class ConnectionRequestLookUp
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserFullName { get; set; } = null!;
    public DateTime CreationTime { get; set; }
    public Guid PhotoId { get; set; }
    public string? JobTitle { get; set; }
}
