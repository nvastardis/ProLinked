using ProLinked.Domain.Entities.Jobs;

namespace ProLinked.Domain.Entities.Recommendations;

public class JobRecommendation: Entity
{
    public Guid UserId { get; init; }
    public Guid AdvertisementId { get; init; }
    public virtual Advertisement? Advertisement { get; init; }

    private JobRecommendation() { }

    public JobRecommendation(
        Guid userId,
        Guid advertisementId)
    {
        UserId = userId;
        AdvertisementId = advertisementId;
    }


    public override object?[] GetKeys()
    {
        return [UserId, AdvertisementId];
    }
}