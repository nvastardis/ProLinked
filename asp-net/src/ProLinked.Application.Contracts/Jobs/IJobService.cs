using ProLinked.Application.Contracts.Filtering;
using ProLinked.Application.Contracts.Jobs.DTOs;
using ProLinked.Application.DTOs;

namespace ProLinked.Application.Contracts.Jobs;

public interface IJobService
{
    Task<PagedAndSortedResultList<AdvertisementDto>> GetListOfJobAdvertisementsAsync(
        JobListFilter input,
        CancellationToken cancellationToken = default);


    Task<PagedAndSortedResultList<AdvertisementDto>> GetRecommendedJobListAsync(
        UserFilterDto filter,
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