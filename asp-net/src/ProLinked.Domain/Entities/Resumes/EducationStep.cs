using System.Collections.ObjectModel;
using ProLinked.Domain.Extensions;
using ProLinked.Domain.Shared.Resumes;
using ProLinked.Domain.Shared.Utils;

namespace ProLinked.Domain.Entities.Resumes;

public class EducationStep : Entity<Guid>
{
    public Guid ResumeId { get; init; }
    public string School { get; internal set; } = null!;
    public string? Degree { get; internal set; }
    public string? FieldOfStudy { get; internal set; }
    public string? Grade { get; internal set; }
    public string? Activities { get; internal set; }
    public string? Description { get; internal set; }
    public DateTime? StartDate { get; internal set; }
    public DateTime? EndDate { get; internal set; }
    public ICollection<EducationStepSkill> RelatedSkills { get; init; }

    private EducationStep(Guid id): base(id){}


    public EducationStep(
        Guid id,
        Guid resumeId,
        string school,
        string? degree,
        string? fieldOfStudy,
        string? grade,
        string? activities,
        string? description,
        DateTime? startDate,
        DateTime? endDate)
        : base(id)
    {
        ResumeId = resumeId;
        SetSchoolName(school);

        SetDegree(degree);
        SetFieldOfStudy(fieldOfStudy);
        SetGrade(grade);
        SetActivities(activities);
        SetDescription(description);

        StartDate = startDate;
        EndDate = endDate;
        RelatedSkills = new Collection<EducationStepSkill>();
    }

    public EducationStep UpdateEducation(
        string? school = null,
        string? degree = null,
        string? fieldOfStudy = null,
        string? grade = null,
        string? activities = null,
        string? description = null,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        if (!school.IsNullOrWhiteSpace())
        {
            SetSchoolName(school!);
        }

        if (!degree.IsNullOrWhiteSpace())
        {
            SetDegree(degree);
        }

        if (!fieldOfStudy.IsNullOrWhiteSpace())
        {
            SetFieldOfStudy(fieldOfStudy);
        }

        if (!grade.IsNullOrWhiteSpace())
        {
            SetGrade(grade);
        }

        if (!activities.IsNullOrWhiteSpace())
        {
            SetActivities(activities);
        }

        if (description is not null)
        {
            SetDescription(description);
        }

        StartDate = startDate;
        EndDate = endDate;

        return this;
    }

    public EducationStep AddRelatedSkill(EducationStepSkill skill)
    {
        RelatedSkills.Add(skill);
        return this;
    }

    private void SetSchoolName(string school)
    {
        School = Check.NotNullOrWhiteSpace(
            school,
            nameof(school),
            EducationConsts.MaxSchoolSize,
            EducationConsts.MinSchoolSize);
    }

    private void SetDegree(string? degree)
    {
        if (!degree.IsNullOrWhiteSpace())
        {
            Degree = Check.Length(
                degree,
                nameof(degree),
                EducationConsts.MaxDegreeSize,
                EducationConsts.MinDegreeSize);
        }
    }

    private void SetFieldOfStudy(string? fieldOfStudy)
    {
        if (!fieldOfStudy.IsNullOrWhiteSpace())
        {
            FieldOfStudy = Check.Length(
                fieldOfStudy,
                nameof(fieldOfStudy),
                EducationConsts.MaxFieldOfStudySize,
                EducationConsts.MinFieldOfStudySize);
        }
    }

    private void SetGrade(string? grade)
    {
        if (!grade.IsNullOrWhiteSpace())
        {
            Grade = Check.Length(
                grade,
                nameof(grade),
                EducationConsts.MaxGradeSize,
                EducationConsts.MinGradeSize);
        }
    }

    private void SetActivities(string? activities)
    {
        if (!activities.IsNullOrWhiteSpace())
        {
            Activities = Check.Length(
                activities,
                nameof(activities),
                EducationConsts.MaxActivitiesSize,
                EducationConsts.MinActivitiesSize);
        }
    }

    private void SetDescription(string? description)
    {
        if (!description.IsNullOrWhiteSpace())
        {
            Description = Check.Length(
                description,
                nameof(description),
                EducationConsts.MaxDescriptionSize,
                EducationConsts.MinDescriptionSize);
        }
    }


}