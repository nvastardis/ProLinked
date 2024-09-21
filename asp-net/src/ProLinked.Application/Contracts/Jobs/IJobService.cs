using ProLinked.Application.DTOs;
using ProLinked.Application.DTOs.Jobs;

namespace ProLinked.Application.Contracts.Jobs;

public interface IJobService
{
    Task<PagedAndSortedResultList<AdvertisementDto>> GetListOfJobAdvertisementsAsync(
        JobListFilter input,
        CancellationToken cancellationToken = default);

    Task<PagedAndSortedResultList<ApplicationDto>> GetListOfApplicationsAsync(
        Guid advertisementId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task CreateJobAdvertisementAsync(
        AdvertisementCUDto input,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task ApplyAsync(
        Guid advertisementId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task CloseAdvertisement(
        Guid advertisementId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task UpdateJobAdvertisementAsync(
        Guid advertisementId,
        AdvertisementCUDto input,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<PagedAndSortedResultList<ApplicationDto>> GetListOfUserApplicationsAsync(
        JobListFilter input,
        CancellationToken cancellationToken = default);

    Task AcceptApplicationAsync(
        Guid applicationId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task RejectApplicationAsync(
        Guid applicationId,
        Guid userId,
        CancellationToken cancellationToken = default);
}