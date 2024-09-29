using ProLinked.Domain.Entities.Jobs;
using ProLinked.Domain.Shared.Resumes;

namespace ProLinked.Domain.Contracts.Jobs;

public interface IJobManager
{
    Task<Advertisement> CreateAdvertisementAsync(
        Guid userId,
        string title,
        string description,
        string company,
        string location,
        EmploymentTypeEnum employmentType,
        WorkArrangementEnum workArrangement,
        CancellationToken cancellationToken = default);

    Task UpdateAdvertisementAsync(
        Guid advertisementId,
        Guid userId,
        string? title = null,
        string? description = null,
        string? company = null,
        string? location = null,
        EmploymentTypeEnum? employmentType = null,
        WorkArrangementEnum? workArrangement = null,
        CancellationToken cancellationToken = default);

    Task<Application> CreateApplicationAsync(
        Guid currentUserId,
        Guid advertisementId,
        CancellationToken cancellationToken = default);

    Task AcceptApplicationAsync(
        Guid currentUserId,
        Guid applicationId,
        CancellationToken cancellationToken = default);

    Task RejectApplicationAsync(
        Guid currentUserId,
        Guid applicationId,
        CancellationToken cancellationToken = default);

    Task CloseAdvertisementAsync(
        Guid currentUserId,
        Guid advertisementId,
        CancellationToken cancellationToken = default);

    Task<Advertisement> GetAdvertisementAsync(
        Guid currentUserId,
        Guid advertisementId,
        CancellationToken cancellationToken = default);
}