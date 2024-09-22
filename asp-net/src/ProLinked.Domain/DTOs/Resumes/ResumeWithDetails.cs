using ProLinked.Domain.Entities.Resumes;

namespace ProLinked.Domain.DTOs.Resumes;

public class ResumeWithDetails
{
    public Guid ResumeId;
    public Guid UserId;
    public string UserFullName = null!;
    public Guid? UserProfilePhotoId;
    public List<EducationStep>? EducationSteps;
    public List<ExperienceStep>? ExperienceSteps;
    public List<Skill>? Skills;
}