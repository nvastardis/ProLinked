using ProLinked.Domain.Shared.Connections;

namespace ProLinked.Application.DTOs.Connections;

public class ConnectionRequestSearchResultDto
{
    public bool Found { get; set; } = false;
    public Guid? RequestId { get; set; }
    public DateTime? CreationTime { get; set; }
    public ConnectionRequestStatus? Status { get; set; }
}