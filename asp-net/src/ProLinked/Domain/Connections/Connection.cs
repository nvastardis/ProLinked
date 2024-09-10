using ProLinked.Domain.Identity;

namespace ProLinked.Domain.Connections;

public class Connection: Entity<Guid>
{
    public Guid UserAId { get; init; }
    public AppUser? UserA { get; init; }
    public Guid UserBId { get; init; }
    public AppUser? UserB { get; init; }
    public DateTime CreationTime { get; init; }


    private Connection(Guid id): base(id) {}

    public Connection(
        Guid id,
        Guid userAId,
        Guid userBId,
        DateTime? creationTime = null)
    : base(id)
    {
        UserAId = userAId;
        UserBId = userBId;
        CreationTime = creationTime ?? DateTime.Now;
    }

}
