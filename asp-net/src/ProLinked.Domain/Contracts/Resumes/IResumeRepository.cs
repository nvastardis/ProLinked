using ProLinked.Domain.Entities.Resumes;

namespace ProLinked.Domain.Repositories.Resumes;

public interface IResumeRepository: IRepository<Resume, Guid>
{
    Task<List<EducationStep>> GetListEducationStepByUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<List<ExperienceStep>> GetListExperienceStepByUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<List<ResumeSkill>> GetListResumeSkillByUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
