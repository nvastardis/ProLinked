using ProLinked.Shared;
using ProLinked.Shared.Resumes;
using ProLinked.Shared.Utils;

namespace ProLinked.Domain.Resumes.Skills;

public class Skill: Entity<Guid>
{
    public string Title { get; set; } = null!;

    private Skill(Guid id): base(id){}

    public Skill(
        Guid id,
        string title)
    : base(id)
    {
        SetTitle(title);
    }

    private Skill SetTitle(string title)
    {
        Title = Check.NotNullOrWhiteSpace(
            title,
            nameof(title),
            SkillConsts.MaxTitleLength,
            SkillConsts.MinTitleLength);

        return this;
    }
}
