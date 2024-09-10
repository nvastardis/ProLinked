using ProLinked.Shared;
using ProLinked.Shared.Jobs;

namespace ProLinked.Domain.Jobs;

public interface IApplicationRepository: IRepository<Application, Guid>
{
    Task<List<Application>> GetListByUserAsync(
        Guid userId,
        DateTime? from = null,
        DateTime? to = null,
        ApplicationStatus status = ApplicationStatus.UNDEFINED,
        bool includeDetails = false,
        string? sorting = null,
        int skipCount = ProLinkedConsts.SkipCountDefaultValue,
        int maxResultCount = ProLinkedConsts.MaxResultCountDefaultValue,
        CancellationToken cancellationToken = default);
}
