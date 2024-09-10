namespace ProLinked.Domain.Resumes.Skills;

public interface ISkillRepository: IRepository<Skill, Guid>
{
    Task<Skill?> FindByTitleAsync(
        string title,
        CancellationToken cancellationToken = default);
}
