using System.Collections.ObjectModel;
using ProLinked.Domain.Extensions;
using ProLinked.Domain.Shared.Jobs;
using ProLinked.Domain.Shared.Resumes;
using ProLinked.Domain.Shared.Utils;

namespace ProLinked.Domain.Entities.Jobs;

public class Advertisement : Entity<Guid>
{
    public Guid CreatorId { get; init; }
    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public string Company { get; private set; } = null!;
    public string Location { get; private set; } = null!;
    public EmploymentTypeEnum EmploymentType { get; private set; }
    public WorkArrangementEnum WorkArrangement { get; private set; }
    public AdvertisementStatus Status { get; private set; }
    public ICollection<Application> Applications { get; init; }
    public DateTime CreationTime { get; init; }
    public DateTime? LastModificationTime { get; private set; }

    private Advertisement(Guid id): base(id){}

    public Advertisement(
        Guid id,
        Guid creatorId,
        string title,
        string description,
        string company,
        string location,
        EmploymentTypeEnum employmentType,
        WorkArrangementEnum workArrangement,
        AdvertisementStatus? status = null,
        DateTime? creationTime = null)
        : base(id)
    {
        CreatorId = creatorId;
        SetTitle(title);
        SetDescription(description);
        SetCompany(company);
        SetLocation(location);
        SetEmploymentType(employmentType);
        SetWorkArrangement(workArrangement);
        Applications = new Collection<Application>();
        SetStatus(status ?? AdvertisementStatus.OPEN);
        CreationTime = creationTime ?? DateTime.Now;
    }

    public Advertisement UpdateAdvertisement(
        string? title,
        string? description,
        string? company,
        string? location,
        EmploymentTypeEnum? employmentType,
        WorkArrangementEnum? workArrangement)
    {
        if (!title.IsNullOrWhiteSpace())
        {
            SetTitle(title!);
        }

        if (!description.IsNullOrWhiteSpace())
        {
            SetDescription(description!);
        }

        if (!company.IsNullOrWhiteSpace())
        {
            SetCompany(company!);
        }

        if (!location.IsNullOrWhiteSpace())
        {
            SetLocation(location!);
        }

        if (employmentType.HasValue)
        {
            EmploymentType = employmentType.Value;
        }

        if (workArrangement.HasValue)
        {
            WorkArrangement = workArrangement.Value;
        }

        LastModificationTime = DateTime.Now;
        return this;
    }

    private Advertisement SetTitle(string title)
    {
        Title = Check.NotNullOrWhiteSpace(
            title,
            nameof(title),
            AdvertisementConsts.MaxTitleLength,
            AdvertisementConsts.MinTitleLength);
        return this;
    }

    private Advertisement SetCompany(string company)
    {
        Company = Check.NotNullOrWhiteSpace(
            company,
            nameof(company),
            AdvertisementConsts.MaxCompanyLength,
            AdvertisementConsts.MinCompanyLength);

        return this;
    }

    private Advertisement SetLocation(string location)
    {
        Location = Check.NotNullOrWhiteSpace(
            location,
            nameof(location),
            AdvertisementConsts.MaxLocationLength,
            AdvertisementConsts.MinLocationLength);

        return this;
    }

    private Advertisement SetDescription(string description)
    {

        Description = Check.NotNullOrWhiteSpace(
            description,
            nameof(description),
            AdvertisementConsts.MaxDescriptionLength,
            AdvertisementConsts.MinDescriptionLength);
        return this;
    }

    private Advertisement SetEmploymentType(EmploymentTypeEnum employmentTypeEnum)
    {
        EmploymentType = employmentTypeEnum;
        return this;
    }

    private Advertisement SetWorkArrangement(WorkArrangementEnum workArrangement)
    {
        WorkArrangement = workArrangement;
        return this;
    }

    public Advertisement AddApplication(Application newApplication)
    {
        Applications.Add(newApplication);
        return this;
    }

    public Advertisement SetStatus(AdvertisementStatus status)
    {
        Status = status;
        return this;
    }
}
