using ProLinked.Domain.Entities.Resumes;
using ProLinked.Domain.Shared.Resumes;

namespace ProLinked.Application.Services.Resumes;

public interface IResumeManager
{
    Task<Resume> CreateResumeAsync(
        Guid currentUserId,
        CancellationToken cancellationToken = default);

    Task<Skill> CreateSkillAsync(
        string title,
        CancellationToken cancellationToken = default);

    Task<Resume> AddEducationStepAsync(
        Guid currentUserId,
        string school,
        string? degree = null,
        string? fieldOfStudy = null,
        string? grade = null,
        string? activitiesAndSocieties = null,
        string? description = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default);

    Task<Resume> UpdateEducationStepAsync(
        Guid currentUserId,
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

    Task<Resume> AddExperienceStepAsync(
        Guid currentUserId,
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

    Task<Resume> UpdateExperienceStepAsync(
        Guid currentUserId,
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

    Task<Resume> MapSkillToResumeAsync(
        Guid currentUserId,
        Guid skillId,
        bool isFollowingSkill,
        CancellationToken cancellationToken = default);

    Task<Resume> MapSkillToEducationAsync(
        Guid currentUserId,
        Guid skillId,
        Guid educationId,
        bool isFollowingSkill,
        CancellationToken cancellationToken = default);

    Task<Resume> MapSkillToExperienceAsync(
        Guid currentUserId,
        Guid skillId,
        Guid experienceId,
        bool isFollowingSkill,
        CancellationToken cancellationToken = default);

    Task<Resume> SetFollowingFlagOnSkillAsync(
        Guid currentUserId,
        Guid skillId,
        bool isFollowing,
        CancellationToken cancellationToken = default);
}
