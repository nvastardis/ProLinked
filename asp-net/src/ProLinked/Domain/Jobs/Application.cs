using ProLinked.Domain.Identity;
using ProLinked.Shared.Jobs;

namespace ProLinked.Domain.Jobs;

public class Application: Entity<Guid>
{
    public Guid AdvertisementId { get; init; }
    public Guid UserId { get; init; }
    public AppUser? User { get; set; }
    public ApplicationStatus Status { get; set; }
    public DateTime CreationTime { get; init; }

    private Application(Guid id): base(id){}


    public Application(
        Guid id,
        Guid advertisementId,
        Guid userId,
        ApplicationStatus? applicationStatus = null,
        DateTime? creationTime = null)
        : base(id)
    {
        AdvertisementId = advertisementId;
        UserId = userId;
        SetStatus(applicationStatus ?? ApplicationStatus.PENDING);
        CreationTime = creationTime ?? DateTime.Now;
    }

    internal Application SetStatus(ApplicationStatus status)
    {
        Status = status;
        return this;
    }
}
