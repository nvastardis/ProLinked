using ProLinked.Domain.Entities.Resumes;
using ProLinked.Domain.Shared.Resumes;

namespace ProLinked.Domain.Contracts.Resumes;

public interface IResumeManager
{
    Task<Resume> CreateResumeAsync(
        Guid currentUserId,
        CancellationToken cancellationToken = default);

    Task<Skill> CreateSkillAsync(
        string title,
        CancellationToken cancellationToken = default);

    Task<EducationStep> AddEducationStepAsync(
        Resume resume,
        string school,
        string? degree = null,
        string? fieldOfStudy = null,
        string? grade = null,
        string? activitiesAndSocieties = null,
        string? description = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default);

    Task UpdateEducationStepAsync(
        Resume resume,
        Guid educationStepId,
        string? school = null,
        string? degree = null,
        string? fieldOfStudy = null,
        string? grade = null,
        string? activitiesAndSocieties = null,
        string? description = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default);

    Task<ExperienceStep> AddExperienceStepAsync(
        Resume resume,
        string title,
        string company,
        EmploymentTypeEnum employmentType,
        bool isEmployed,
        string location,
        WorkArrangementEnum workArrangement,
        string? description = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default);

    Task UpdateExperienceStepAsync(
        Resume resume,
        Guid experienceStepId,
        string? title = null,
        string? company = null,
        EmploymentTypeEnum? employmentType = null,
        bool? isEmployed = null,
        string? location = null,
        WorkArrangementEnum? workArrangement = null,
        string? description = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default);

    Task<ResumeSkill> MapSkillToResumeAsync(
        Resume resume,
        Guid skillId,
        bool isFollowingSkill,
        CancellationToken cancellationToken = default);

    Task<EducationStepSkill> MapSkillToEducationAsync(
        EducationStep step,
        Guid skillId,
        bool isFollowingSkill,
        CancellationToken cancellationToken = default);

    Task<ExperienceStepSkill> MapSkillToExperienceAsync(
        ExperienceStep step,
        Guid skillId,
        bool isFollowingSkill,
        CancellationToken cancellationToken = default);

    Task SetFollowingFlagOnSkillAsync(
        ResumeSkill resumeSkill,
        bool isFollowing,
        CancellationToken cancellationToken = default);

    Task<Resume> GetResumeAsync(
        Guid resumeId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task DeleteResumeAsync(
        Resume resume,
        CancellationToken cancellationToken = default);

    Task<ResumeSkill> GetResumeSkillAsync(
        Guid resumeId,
        Guid skillId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task DeleteResumeSkillAsync(
        ResumeSkill resumeSkill,
        CancellationToken cancellationToken = default);

    Task<EducationStep> GetEducationStepAsync(
        Guid resumeId,
        Guid stepId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task DeleteEducationStepAsync(
        EducationStep educationStep,
        CancellationToken cancellationToken = default);

    Task DeleteEducationStepSkillAsync(
        EducationStep educationStep,
        Guid skillId,
        CancellationToken cancellationToken = default);

    Task<ExperienceStep> GetExperienceStepAsync(
        Guid resumeId,
        Guid stepId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task DeleteExperienceStepAsync(
        ExperienceStep experienceStep,
        CancellationToken cancellationToken = default);

    Task DeleteExperienceStepSkillAsync(
        ExperienceStep experienceStep,
        Guid skillId,
        CancellationToken cancellationToken = default);
}