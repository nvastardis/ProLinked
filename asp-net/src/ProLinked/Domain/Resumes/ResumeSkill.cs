namespace ProLinked.Domain.Resumes;

public class ResumeSkill: Entity
{
    public Guid ResumeId { get; init; }
    public Guid SkillId { get; init; }
    public bool IsFollowingSkill { get; set; }

    private ResumeSkill(){}

    public ResumeSkill(
        Guid resumeId,
        Guid skillId,
        bool isFollowingSkill)
    {
        ResumeId = resumeId;
        SkillId = skillId;
        IsFollowingSkill = isFollowingSkill;
    }

    internal ResumeSkill SetFollowingFlag(bool isFollowing)
    {
        IsFollowingSkill = isFollowing;
        return this;
    }

    public override object?[] GetKeys()
    {
        return [ResumeId, SkillId];
    }
}
