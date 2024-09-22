using ProLinked.Domain.DTOs.Resumes;
using ProLinked.Domain.Entities.Resumes;

namespace ProLinked.Domain.Contracts.Resumes;

public interface IResumeRepository: IRepository<Resume, Guid>
{
    Task<List<EducationStep>> GetListEducationStepByUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<List<EducationStep>> GetListEducationStepAsync(
        Guid resumeId,
        CancellationToken cancellationToken = default);

    Task<List<ExperienceStep>> GetListExperienceStepByUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<List<ExperienceStep>> GetListExperienceStepAsync(
        Guid resumeId,
        CancellationToken cancellationToken = default);

    Task<List<Skill>> GetListResumeSkillByUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<List<Skill>> GetListResumeSkillAsync(
        Guid resumeId,
        CancellationToken cancellationToken = default);

    Task<ResumeWithDetails> GetWithDetailsAsync(
        Guid resumeId,
        CancellationToken cancellationToken);
}