using AutoMapper;
using Microsoft.Extensions.Logging;
using ProLinked.Application.Contracts.Jobs;
using ProLinked.Application.Contracts.Jobs.DTOs;
using ProLinked.Application.DTOs;
using ProLinked.Domain.Contracts.Jobs;
using ProLinked.Domain.Entities.Jobs;
using JobApplication = ProLinked.Domain.Entities.Jobs.Application;

namespace ProLinked.Application.Services.Jobs;

public class JobService: ProLinkedServiceBase, IJobService
{
    public readonly IJobManager JobManager;
    public readonly IAdvertisementRepository AdvertisementRepository;
    public readonly IApplicationRepository ApplicationRepository;

    public JobService(
        IMapper objectMapper,
        IJobManager jobManager,
        IAdvertisementRepository advertisementRepository,
        IApplicationRepository applicationRepository)
        : base(objectMapper)
    {
        JobManager = jobManager;
        AdvertisementRepository = advertisementRepository;
        ApplicationRepository = applicationRepository;
    }

    public async Task<PagedAndSortedResultList<AdvertisementDto>> GetListOfJobAdvertisementsAsync(
        JobListFilter input,
        CancellationToken cancellationToken = default)
    {
        var queryResult = await AdvertisementRepository.GetListByUserAsync(
            input.UserId,
            input.From,
            input.To,
            input.AdvertisementStatus,
            input.IncludeDetails,
            input.Sorting,
            input.SkipCount,
            input.MaxResultCount,
            cancellationToken);

        var items = ObjectMapper.Map<List<Advertisement>, List<AdvertisementDto>>(queryResult);
        var itemCount = items.Count;

        return new PagedAndSortedResultList<AdvertisementDto>(itemCount,items.AsReadOnly());
    }

    public async Task<PagedAndSortedResultList<ApplicationDto>> GetListOfApplicationsAsync(
        Guid advertisementId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var advertisement = await JobManager.GetAdvertisementAsync(
            userId,
            advertisementId,
            cancellationToken);

        var items = ObjectMapper.Map<List<JobApplication>, List<ApplicationDto>>(
                advertisement.Applications.ToList()
            );
        var itemCount = items.Count;

        return new PagedAndSortedResultList<ApplicationDto>(itemCount, items.AsReadOnly());
    }

    public async Task CreateJobAdvertisementAsync(
        AdvertisementCUDto input,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        await JobManager.CreateAdvertisementAsync(
            userId,
            input.Title,
            input.Description,
            input.Company,
            input.Location,
            input.EmploymentType,
            input.WorkArrangement,
            cancellationToken);
    }

    public async Task ApplyAsync(
        Guid advertisementId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        await JobManager.CreateApplicationAsync(
            userId,
            advertisementId,
            cancellationToken);
    }

    public async Task CloseAdvertisement(
        Guid advertisementId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        await JobManager.CloseAdvertisementAsync(userId, advertisementId, cancellationToken);
    }

    public async Task UpdateJobAdvertisementAsync(
        Guid id,
        AdvertisementCUDto input,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        await JobManager.UpdateAdvertisementAsync(
            id,
            userId,
            input.Title,
            input.Description,
            input.Company,
            input.Location,
            input.EmploymentType,
            input.WorkArrangement,
            cancellationToken);
    }

    public async Task<PagedAndSortedResultList<ApplicationDto>> GetListOfUserApplicationsAsync(
        JobListFilter input,
        CancellationToken cancellationToken = default)
    {
        var queryResult = await ApplicationRepository.GetListByUserAsync(
            input.UserId,
            input.From,
            input.To,
            input.ApplicationStatus,
            input.IncludeDetails,
            input.Sorting,
            input.SkipCount,
            input.MaxResultCount,
            cancellationToken);

        var items = ObjectMapper.Map<List<JobApplication>, List<ApplicationDto>>(queryResult);
        var itemCount = items.Count;
        return new PagedAndSortedResultList<ApplicationDto>(itemCount, items);
    }

    public async Task AcceptApplicationAsync(
        Guid applicationId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        await JobManager.AcceptApplicationAsync(
            userId,
            applicationId,
            cancellationToken);
    }

    public async Task RejectApplicationAsync(
        Guid applicationId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        await JobManager.RejectApplicationAsync(
            userId,
            applicationId,
            cancellationToken);
    }
}