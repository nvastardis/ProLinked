using ProLinked.Domain.Entities.Connections;
using ProLinked.Domain.Shared.Connections;
using ConnectionInfo = ProLinked.Domain.DTOs.Connections.ConnectionInfo;

namespace ProLinked.Domain.Contracts.Connections;

public interface IConnectionManager
{
    Task<ConnectionRequest> CreateConnectionRequestAsync(
        Guid senderId,
        Guid targetId,
        CancellationToken cancellationToken = default);

    Task<ConnectionInfo> UpdateRequestAsync(
        ConnectionRequest request,
        ConnectionRequestStatus status,
        CancellationToken cancellationToken = default);

    Task<Connection> GetConnectionAsync(
        Guid connectionId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<ConnectionRequest> GetRequestAsync(
        Guid requestId,
        Guid userId,
        CancellationToken cancellationToken = default);


}