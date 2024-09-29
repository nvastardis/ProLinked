using ProLinked.Domain.Contracts.Jobs;
using ProLinked.Domain.Entities.Jobs;
using ProLinked.Domain.Shared;
using ProLinked.Domain.Shared.Exceptions;
using ProLinked.Domain.Shared.Jobs;
using ProLinked.Domain.Shared.Resumes;

namespace ProLinked.Domain.Services;

public class JobManager: IJobManager
{
    private readonly IAdvertisementRepository _advertisementRepository;
    private readonly IApplicationRepository _applicationRepository;
    public JobManager(
        IAdvertisementRepository advertisementRepository,
        IApplicationRepository applicationRepository)
    {
        _advertisementRepository = advertisementRepository;
        _applicationRepository = applicationRepository;
    }

    public async Task<Advertisement> CreateAdvertisementAsync(
        Guid userId,
        string title,
        string description,
        string company,
        string location,
        EmploymentTypeEnum employmentType,
        WorkArrangementEnum workArrangement,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var newAdvertisement = new Advertisement(
            Guid.NewGuid(),
            userId,
            title,
            description,
            company,
            location,
            employmentType,
            workArrangement);

        await _advertisementRepository.InsertAsync(newAdvertisement, autoSave: true, cancellationToken);
        return newAdvertisement;
    }

    public async Task UpdateAdvertisementAsync(
        Guid advertisementId,
        Guid userId,
        string? title = null,
        string? description = null,
        string? company = null,
        string? location = null,
        EmploymentTypeEnum? employmentType = null,
        WorkArrangementEnum? workArrangement = null,
        CancellationToken cancellationToken = default)
    {
        var adv = await GetAdvertisementAsync(userId, advertisementId, cancellationToken);
        if (adv.Status != AdvertisementStatus.OPEN)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.JobAdvertisementAlreadyClosed)
                .WithData(nameof(Advertisement.Id), advertisementId);
        }

        if (adv.Applications.Any())
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.JobAdvertisementHasApplications)
                .WithData(nameof(Advertisement.Id), advertisementId);
        }

        cancellationToken.ThrowIfCancellationRequested();
        adv.UpdateAdvertisement(
            title,
            description,
            company,
            location,
            employmentType,
            workArrangement);
        await _advertisementRepository.UpdateAsync(adv, autoSave: true, cancellationToken);
    }

    public async Task<Application> CreateApplicationAsync(
        Guid currentUserId,
        Guid advertisementId,
        CancellationToken cancellationToken = default)
    {
        var advertisement = await GetAdvertisementAsync(currentUserId, advertisementId, cancellationToken);
        if (advertisement.Status == AdvertisementStatus.CLOSED)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.JobApplicationPeriodClosed);
        }
        if (advertisement.Applications.Any(e => e.UserId == currentUserId))
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.UserAlreadyApplied);
        }

        cancellationToken.ThrowIfCancellationRequested();
        var newApplication = new Application(
            Guid.NewGuid(),
            advertisementId,
            currentUserId);
        advertisement.AddApplication(newApplication);
        await _applicationRepository.InsertAsync(newApplication, autoSave: true, cancellationToken);
        return newApplication;
    }

    public async Task AcceptApplicationAsync(
        Guid currentUserId,
        Guid applicationId,
        CancellationToken cancellationToken = default)
    {
        var application = await _applicationRepository.FindAsync(applicationId, false, cancellationToken);
        if (application is null)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.JobApplicationNotFound);
        }

        _ = await GetAdvertisementAsync(currentUserId, application.AdvertisementId, cancellationToken);

        cancellationToken.ThrowIfCancellationRequested();
        application.SetStatus(ApplicationStatus.ACCEPTED_FOR_INTERVIEW);
        await _applicationRepository.UpdateAsync(application, autoSave: true, cancellationToken);
    }

    public async Task RejectApplicationAsync(
        Guid currentUserId,
        Guid applicationId,
        CancellationToken cancellationToken = default)
    {
        var application = await _applicationRepository.FindAsync(applicationId, false, cancellationToken);
        if (application is null)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.JobApplicationNotFound);
        }

        _ = await GetAdvertisementAsync(currentUserId, application.AdvertisementId, cancellationToken);

        cancellationToken.ThrowIfCancellationRequested();
        application.SetStatus(ApplicationStatus.REJECTED);
        await _applicationRepository.UpdateAsync(application, autoSave: true, cancellationToken);
    }

    public async Task CloseAdvertisementAsync(
        Guid currentUserId,
        Guid advertisementId,
        CancellationToken cancellationToken = default)
    {
        var advertisement = await GetAdvertisementAsync(currentUserId, advertisementId, cancellationToken);
        if (advertisement.Status == AdvertisementStatus.CLOSED)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.JobAdvertisementAlreadyClosed);
        }

        cancellationToken.ThrowIfCancellationRequested();
        advertisement.SetStatus(AdvertisementStatus.CLOSED);
        await _advertisementRepository.UpdateAsync(advertisement, autoSave: true, cancellationToken);


        foreach (var application in advertisement.Applications)
        {
            if (application.Status == ApplicationStatus.PENDING)
            {
                application.SetStatus(ApplicationStatus.REJECTED);
                await _applicationRepository.UpdateAsync(application, autoSave: true, cancellationToken);
            }
        }
    }

    public async Task<Advertisement> GetAdvertisementAsync(Guid currentUserId, Guid advertisementId, CancellationToken cancellationToken = default)
    {
        var advertisement = await _advertisementRepository.FindAsync(advertisementId, includeDetails:true, cancellationToken);
        if (advertisement is null)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.JobAdvertisementNotFound);
        }
        if (advertisement.CreatorId != currentUserId)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.UserIsNotAdvertiser);
        }

        return advertisement;
    }
}