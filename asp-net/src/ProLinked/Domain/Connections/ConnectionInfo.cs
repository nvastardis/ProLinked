namespace ProLinked.Domain.Connections;

public class ConnectionInfo
{
    public ConnectionRequest Request { get; set; } = null!;
    public Connection? Connection { get; set; }
}
