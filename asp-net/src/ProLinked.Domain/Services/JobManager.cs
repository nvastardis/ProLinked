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

        return await Task.FromResult(newAdvertisement);
    }

    public async Task<Advertisement> UpdateAdvertisementAsync(
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
        var adv = await _advertisementRepository.FindAsync(advertisementId, includeDetails: true, cancellationToken);
        if (adv is null)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.JobAdvertisementNotFound)
                .WithData(nameof(Advertisement.Id), advertisementId);
        }

        if (adv.CreatorId != userId)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.UserIsNotAdvertiser)
                .WithData(nameof(Advertisement.Id), adv.Id)
                .WithData(nameof(Advertisement.CreatorId), adv.CreatorId);
        }

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

        return adv;
    }

    public async Task<Advertisement> CreateApplicationAsync(
        Guid currentUserId,
        Guid advertisementId,
        CancellationToken cancellationToken = default)
    {
        var advertisement = await _advertisementRepository.FindAsync(advertisementId, includeDetails:true, cancellationToken);
        if (advertisement is null)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.JobAdvertisementNotFound);
        }
        if (advertisement.CreatorId == currentUserId)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.AdvertiserCannotApply);
        }
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
        return advertisement;
    }

    public async Task<Advertisement> AcceptApplicationAsync(
        Guid currentUserId,
        Guid applicationId,
        CancellationToken cancellationToken = default)
    {
        var application = await _applicationRepository.FindAsync(applicationId, false, cancellationToken);
        if (application is null)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.JobApplicationNotFound);
        }

        var advertisement = await _advertisementRepository.GetAsync(application.AdvertisementId, cancellationToken: cancellationToken);
        if (advertisement.CreatorId != currentUserId)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.UserIsNotAdvertiser);
        }

        cancellationToken.ThrowIfCancellationRequested();
        application.SetStatus(ApplicationStatus.ACCEPTED_FOR_INTERVIEW);

        return advertisement;
    }

    public async Task<Advertisement> CloseAdvertisementAsync(
        Guid currentUserId,
        Guid advertisementId,
        CancellationToken cancellationToken = default)
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
        if (advertisement.Status == AdvertisementStatus.CLOSED)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.JobAdvertisementAlreadyClosed);
        }

        cancellationToken.ThrowIfCancellationRequested();
        advertisement.SetStatus(AdvertisementStatus.CLOSED);

        foreach (var application in advertisement.Applications)
        {
            if (application.Status == ApplicationStatus.PENDING)
            {
                application.SetStatus(ApplicationStatus.REJECTED);
            }
        }
        return advertisement;
    }
}