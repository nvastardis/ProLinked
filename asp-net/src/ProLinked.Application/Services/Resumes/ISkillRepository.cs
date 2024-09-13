using ProLinked.Domain;
using ProLinked.Domain.Entities.Resumes;

namespace ProLinked.Application.Services.Resumes;

public interface ISkillRepository: IRepository<Skill, Guid>
{
    Task<Skill?> FindByTitleAsync(
        string title,
        CancellationToken cancellationToken = default);
}
