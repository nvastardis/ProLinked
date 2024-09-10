using ProLinked.Domain.Resumes.Education;
using ProLinked.Domain.Resumes.Experience;

namespace ProLinked.Domain.Resumes;

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
