namespace ProLinked.Application.Contracts.Resumes.DTOs;

public class ResumeDto
{
    public Guid ResumeId;
    public List<EducationStepDto>? EducationSteps;
    public List<ExperienceStepDto>? ExperienceSteps;
    public List<SkillDto>? Skills;
}