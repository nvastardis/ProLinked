using ProLinked.Domain.Shared.Resumes;
using ProLinked.Domain.Shared.Utils;

namespace ProLinked.Domain.Entities.Resumes;

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
