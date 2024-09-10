using System.Collections.ObjectModel;
using ProLinked.Shared.Extensions;
using ProLinked.Shared.Resumes;
using ProLinked.Shared.Utils;

namespace ProLinked.Domain.Resumes.Experience;

public class ExperienceStep: Entity<Guid>
{
    public Guid ResumeId { get; init; }
    public string Title { get; internal set; } = null!;
    public string Company { get; internal set; } = null!;
    public EmploymentTypeEnum EmploymentType { get; internal set; }
    public bool IsEmployed { get; internal set; }
    public string Location { get; internal set; } = null!;
    public WorkArrangementEnum WorkArrangement { get; internal set; }
    public string? Description { get; internal set; }
    public DateTime? StartDate { get; internal set; }
    public DateTime? EndDate { get; internal set; }
    public ICollection<ExperienceStepSkill> RelatedSkills { get; init; }

    private ExperienceStep(Guid id): base(id){}

    public ExperienceStep(
        Guid id,
        Guid resumeId,
        string title,
        string company,
        EmploymentTypeEnum employmentType,
        bool isEmployed,
        string location,
        WorkArrangementEnum workArrangement,
        string? description,
        DateTime? startDate,
        DateTime? endDate)
        : base(id)
    {
        ResumeId = resumeId;
        SetTitle(title);

        SetCompany(company);
        SetLocation(location);
        SetDescription(description);

        EmploymentType = employmentType;
        WorkArrangement = workArrangement;
        IsEmployed = isEmployed;

        StartDate = startDate;
        EndDate = endDate;

        RelatedSkills = new Collection<ExperienceStepSkill>();
    }

    internal ExperienceStep UpdateExperience(
        string? title = null,
        string? company = null,
        EmploymentTypeEnum? employmentType = null,
        bool? isEmployed = null,
        string? location = null,
        WorkArrangementEnum? workArrangement = null,
        string? description = null,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        if (!title.IsNullOrWhiteSpace())
        {
            SetTitle(title!);
        }

        if (!company.IsNullOrWhiteSpace())
        {
            SetCompany(company!);
        }

        if (employmentType.HasValue)
        {
            EmploymentType = employmentType.Value;
        }

        if (isEmployed.HasValue)
        {
            IsEmployed = isEmployed.Value;
        }

        if (!location.IsNullOrWhiteSpace())
        {
            SetLocation(location!);
        }

        if (workArrangement.HasValue)
        {
            WorkArrangement = workArrangement.Value;
        }

        if (description is not null)
        {
            SetDescription(description);
        }

        StartDate = startDate;
        EndDate = endDate;

        return this;
    }

    internal ExperienceStep AddRelatedSkill(ExperienceStepSkill skill)
    {
        RelatedSkills.Add(skill);
        return this;
    }

    private void SetTitle(string title)
    {
        Title = Check.NotNullOrWhiteSpace(
            title,
            nameof(title),
            ExperienceConsts.MaxTitleSize,
            ExperienceConsts.MinTitleSize);
    }

    private void SetCompany(string company)
    {
        Company = Check.NotNullOrWhiteSpace(
            company,
            nameof(company),
            ExperienceConsts.MaxCompanySize,
            ExperienceConsts.MinCompanySize);
    }

    private void SetLocation(string location)
    {
        Location = Check.NotNullOrWhiteSpace(
            location,
            nameof(location),
            ExperienceConsts.MaxLocationSize,
            ExperienceConsts.MinLocationSize);
    }

    private void SetDescription(string? description)
    {
        if (!description.IsNullOrWhiteSpace())
        {
            Description = Check.Length(
                description,
                nameof(description),
                ExperienceConsts.MaxDescriptionSize,
                ExperienceConsts.MinDescriptionSize);
        }
    }
}
