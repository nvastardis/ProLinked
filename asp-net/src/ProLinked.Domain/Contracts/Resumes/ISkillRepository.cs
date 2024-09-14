using ProLinked.Domain.Entities.Resumes;

namespace ProLinked.Domain.Repositories.Resumes;

public interface ISkillRepository: IRepository<Skill, Guid>
{
    Task<Skill?> FindByTitleAsync(
        string title,
        CancellationToken cancellationToken = default);
}
