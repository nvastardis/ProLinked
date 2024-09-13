using ProLinked.Domain.Entities.Identity;
using ProLinked.Domain.Shared;
using ProLinked.Domain.Shared.Connections;
using ProLinked.Domain.Shared.Exceptions;

namespace ProLinked.Domain.Entities.Connections;

public class ConnectionRequest: Entity<Guid>
{
    public Guid SenderId { get; init; }
    public virtual AppUser? Sender { get; init; }
    public Guid TargetId { get; init; }
    public virtual AppUser? Target { get; init; }
    public ConnectionRequestStatus Status { get; private set; }
    public DateTime CreationTime { get; init; }

    private ConnectionRequest(Guid id): base(id) {}

    public ConnectionRequest(
        Guid id,
        Guid senderId,
        Guid targetId,
        ConnectionRequestStatus? status = null,
        DateTime? creationTime = null)
        : base(id)
    {
        SenderId = senderId;
        TargetId = targetId;
        SetStatus(status ?? ConnectionRequestStatus.PENDING);
        CreationTime = creationTime ?? DateTime.Now;
    }

    public ConnectionRequest SetStatus(ConnectionRequestStatus status)
    {
        if (status == ConnectionRequestStatus.UNDEFINED)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.ConnectionRequestInvalidStatus);
        }
        Status = status;
        return this;
    }
}
