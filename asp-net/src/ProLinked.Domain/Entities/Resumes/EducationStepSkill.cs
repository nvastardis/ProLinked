namespace ProLinked.Domain.Entities.Resumes;

public class EducationStepSkill: Entity
{
    public Guid EducationStepId { get; init; }
    public Guid SkillId { get; init; }

    private EducationStepSkill(){}

    public EducationStepSkill(
        Guid educationStepId,
        Guid skillId)
    {
        SkillId = skillId;
        EducationStepId = educationStepId;
    }

    public override object?[] GetKeys()
    {
        return [EducationStepId, SkillId];
    }
}
