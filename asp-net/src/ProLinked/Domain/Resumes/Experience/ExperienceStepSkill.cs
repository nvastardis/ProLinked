namespace ProLinked.Domain.Resumes.Experience;

public class ExperienceStepSkill: Entity
{
    public Guid ExperienceStepId { get; init; }
    public Guid SkillId { get; init; }

    private ExperienceStepSkill(){}
    public ExperienceStepSkill(
        Guid experienceStepId,
        Guid skillId)
    {
        SkillId = skillId;
        ExperienceStepId = experienceStepId;
    }

    public override object?[] GetKeys()
    {
        return [ExperienceStepId, SkillId];
    }
}
