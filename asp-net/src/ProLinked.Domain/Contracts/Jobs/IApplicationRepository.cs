using ProLinked.Domain.Shared.Jobs;
using ProLinkedApplication = ProLinked.Domain.Entities.Jobs.Application;

namespace ProLinked.Domain.Contracts.Jobs;

public interface IApplicationRepository: IRepository<ProLinkedApplication, Guid>
{
    Task<List<ProLinkedApplication>> GetListByUserAsync(
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