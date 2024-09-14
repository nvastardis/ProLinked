using ProLinked.Domain.Entities.Connections;

namespace ProLinked.Domain.DTOs.Connections;

public class ConnectionInfo
{
    public ConnectionRequest Request { get; set; } = null!;
    public Connection? Connection { get; set; }
}
