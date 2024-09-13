using ProLinked.Domain;
using ProLinked.Domain.Entities.Jobs;
using ProLinked.Domain.Shared;
using ProLinked.Domain.Shared.Jobs;

namespace ProLinked.Application.Services.Jobs;

public interface IAdvertisementRepository: IRepository<Advertisement, Guid>
{
    Task<List<Advertisement>> GetListByUserAsync(
        Guid userId,
        DateTime? from = null,
        DateTime? to = null,
        AdvertisementStatus status = AdvertisementStatus.UNDEFINED,
        bool includeDetails = false,
        string? sorting = null,
        int skipCount = ProLinkedConsts.SkipCountDefaultValue,
        int maxResultCount = ProLinkedConsts.MaxResultCountDefaultValue,
        CancellationToken cancellationToken = default);
}
