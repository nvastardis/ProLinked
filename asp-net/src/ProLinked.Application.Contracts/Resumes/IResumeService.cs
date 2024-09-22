using ProLinked.Application.Contracts.Resumes.DTOs;
using ProLinked.Application.DTOs;

namespace ProLinked.Application.Contracts.Resumes;

public interface IResumeService
{
    /* Resume */
    Task<ResumeDto> GetResumeAsync(
        Guid resumeId,
        CancellationToken cancellationToken = default);

    Task<PagedAndSortedResultList<SkillDto>> GetResumeSkillsAsync(
        Guid resumeId,
        CancellationToken cancellationToken = default);

    Task CreateResumeAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task CreateResumeSkillAsync(
        SkillToResumeDto input,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task DeleteResumeSkillAsync(
        SkillToResumeDto input,
        Guid userId,
        CancellationToken cancellationToken = default);

    /* Education */
    Task<PagedAndSortedResultList<EducationStepDto>> GetEducationStepsAsync(
        Guid resumeId,
        CancellationToken cancellationToken = default);

    Task CreateEducationStepAsync(
        EducationStepCUDto input,
        Guid resumeId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task UpdateEducationStepAsync(
        EducationStepCUDto input,
        Guid educationStepId,
        Guid resumeId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task MapSkillToEducationStepAsync(
        SkillToStepMapDto input,
        Guid resumeId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task DeleteSkillFromEducationStepAsync(
        SkillToStepMapDto input,
        Guid resumeId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task DeleteEducationStepAsync(
        Guid educationStepId,
        Guid resumeId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /* Experience */
    Task<PagedAndSortedResultList<ExperienceStepDto>> GetExperienceStepsAsync(
        Guid resumeId,
        CancellationToken cancellationToken = default);

    Task CreateExperienceStepAsync(
        ExperienceStepCUDto input,
        Guid resumeId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task UpdateExperienceStepAsync(
        ExperienceStepCUDto input,
        Guid stepId,
        Guid resumeId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task MapSkillToExperienceStepAsync(
        SkillToStepMapDto input,
        Guid resumeId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task DeleteSkillFromExperienceStepAsync(
        SkillToStepMapDto input,
        Guid resumeId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task DeleteExperienceStepAsync(
        Guid stepId,
        Guid resumeId,
        Guid userId,
        CancellationToken cancellationToken = default);
}