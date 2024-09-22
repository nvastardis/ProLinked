namespace ProLinked.Application.Contracts.Resumes.DTOs;

public class SkillToStepMapDto
{
    public Guid SkillId { get; set; }
    public Guid StepId { get; set; }
    public bool IsFollowingSkill { get; set; } = true;
}