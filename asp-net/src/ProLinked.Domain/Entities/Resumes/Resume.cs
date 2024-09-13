using System.Collections.ObjectModel;

namespace ProLinked.Domain.Entities.Resumes;

public class Resume: Entity<Guid>
{
    public Guid UserId { get; init; }
    public ICollection<EducationStep> Education { get; init; }
    public ICollection<ExperienceStep> Experience { get; init; }
    public ICollection<ResumeSkill> ResumeSkills { get; init; }

    private Resume(Guid id): base(id){}

    public Resume(
        Guid id,
        Guid userId)
    : base(id)
    {
        UserId = userId;
        Education = new Collection<EducationStep>();
        Experience = new Collection<ExperienceStep>();
        ResumeSkills = new Collection<ResumeSkill>();
    }

    public Resume AddEducationStep(EducationStep educationStep)
    {
        Education.Add(educationStep);
        return this;
    }

    public Resume AddExperienceStep(ExperienceStep experienceStep)
    {
        Experience.Add(experienceStep);
        return this;
    }

    public Resume AddResumeSkill(ResumeSkill resumeSkill)
    {
        ResumeSkills.Add(resumeSkill);
        return this;
    }
}
