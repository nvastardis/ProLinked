using ProLinked.Domain.Resumes.Education;
using ProLinked.Domain.Resumes.Experience;
using ProLinked.Domain.Resumes.Skills;
using ProLinked.Shared;
using ProLinked.Shared.Exceptions;
using ProLinked.Shared.Resumes;

namespace ProLinked.Domain.Resumes;

public class ResumeManager: IDomainService
{
    private readonly ISkillRepository _skillRepository;
    private readonly IResumeRepository _resumeRepository;

    public ResumeManager(
        ISkillRepository skillRepository,
        IResumeRepository resumeRepository)
    {
        _skillRepository = skillRepository;
        _resumeRepository = resumeRepository;
    }

    public async Task<Resume> CreateResumeAsync(
        Guid currentUserId)
    {
        var resume = await _resumeRepository.FindAsync(e => e.UserId == currentUserId);
        if (resume is not null)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.ResumeAlreadyExists);
        }

        var newResume = new Resume(
            Guid.NewGuid(),
            currentUserId);

        return newResume;
    }

    public async Task<Skill> CreateSkillAsync(string title)
    {
        var skillFound = await _skillRepository.FindByTitleAsync(title);
        if (skillFound is not null)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.SkillAlreadyExists)
                .WithData("Existing skillId", skillFound.Id);
        }

        var newSkill = new Skill(
            Guid.NewGuid(),
            title);

        return newSkill;
    }

    public async Task<Resume> AddEducationStepAsync(
        Guid currentUserId,
        string school,
        string? degree = null,
        string? fieldOfStudy = null,
        string? grade = null,
        string? activitiesAndSocieties = null,
        string? description = null,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        var resume = await FindResumeByUserAsync(currentUserId);

        var newEducationStep = new EducationStep(
            Guid.NewGuid(),
            resume.Id,
            school,
            degree,
            fieldOfStudy,
            grade,
            activitiesAndSocieties,
            description,
            startDate,
            endDate);

        resume.AddEducationStep(newEducationStep);

        return resume;
    }

    public async Task<Resume> UpdateEducationStepAsync(
        Guid currentUserId,
        Guid educationStepId,
        string? school = null,
        string? degree = null,
        string? fieldOfStudy = null,
        string? grade = null,
        string? activitiesAndSocieties = null,
        string? description = null,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        var resume = await FindResumeByUserAsync(currentUserId);
        var step = resume.Education.FirstOrDefault(e => e.Id == educationStepId);
        if (step is null)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.EducationStepNotFound);
        }

        step.UpdateEducation(
            school,
            degree,
            fieldOfStudy,
            grade,
            activitiesAndSocieties,
            description,
            startDate,
            endDate);
        return resume;
    }

    public async Task<Resume> AddExperienceStepAsync(
        Guid currentUserId,
        string title,
        string company,
        EmploymentTypeEnum employmentType,
        bool isEmployed,
        string location,
        WorkArrangementEnum workArrangement,
        string? description = null,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        var resume = await FindResumeByUserAsync(currentUserId);

        var newExperience = new ExperienceStep(
            Guid.NewGuid(),
            resume.Id,
            title,
            company,
            employmentType,
            isEmployed,
            location,
            workArrangement,
            description,
            startDate,
            endDate);

        resume.AddExperienceStep(newExperience);
        return resume;
    }

    public async Task<Resume> UpdateExperienceStepAsync(
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
        DateTime? endDate = null)
    {
        var resume = await FindResumeByUserAsync(currentUserId);
        var step = resume.Experience.FirstOrDefault(e => e.Id == experienceStepId);
        if (step is null)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.ExperienceStepNotFound);
        }

        step.UpdateExperience(
            title,
            company,
            employmentType,
            isEmployed,
            location,
            workArrangement,
            description,
            startDate,
            endDate);
        return resume;
    }

    public async Task<Resume> MapSkillToResumeAsync(
        Guid currentUserId,
        Guid skillId,
        bool isFollowingSkill)
    {
        var resume = await FindResumeByUserAsync(currentUserId);
        var skill = await _skillRepository.FindAsync(skillId);
        if (skill is null)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.SkillNotFound);
        }
        if (resume.ResumeSkills.Any(e => e.SkillId == skillId))
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.SkillAlreadyMappedToResume);
        }

        return AddSkillToResume(resume, skill, isFollowingSkill);
    }

    public async Task<Resume> MapSkillToEducationAsync(
        Guid currentUserId,
        Guid skillId,
        Guid educationId,
        bool isFollowingSkill)
    {
        var resume = await MapSkillToResumeStepDefaultChecks(currentUserId, skillId, isFollowingSkill);
        var education = resume.Education.FirstOrDefault(e => e.Id == educationId);
        if (education is null)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.EducationStepNotFound)
                .WithData("EducationStepId", educationId);
        }
        if (education.RelatedSkills.Any(e => e.SkillId == skillId))
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.SKillAlreadyMappedToEducationStep);
        }

        var newEducationSkill = new EducationStepSkill(educationId, skillId);
        education.AddRelatedSkill(newEducationSkill);

        return resume;
    }

    public async Task<Resume> MapSkillToExperienceAsync(
        Guid currentUserId,
        Guid skillId,
        Guid experienceId,
        bool isFollowingSkill)
    {
        var resume = await MapSkillToResumeStepDefaultChecks(currentUserId, skillId, isFollowingSkill);
        var experience = resume.Experience.FirstOrDefault(e => e.Id == experienceId);
        if (experience is null)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.ExperienceStepNotFound)
                .WithData("ExperienceId", experienceId);
        }
        if (experience.RelatedSkills.Any(e => e.SkillId == skillId))
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.SKillAlreadyMappedToExperienceStep);
        }
        var newExperienceSkill = new ExperienceStepSkill(experienceId, skillId);
        experience.AddRelatedSkill(newExperienceSkill);

        return resume;
    }

    public async Task<Resume> SetFollowingFlagOnSkill(
        Guid currentUserId,
        Guid skillId,
        bool isFollowing)
    {
        var resume = await FindResumeByUserAsync(currentUserId);
        var relatedSKill = resume.ResumeSkills.FirstOrDefault(e => e.SkillId == skillId);
        if (relatedSKill is null)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.SkillNotMappedInResume);
        }
        relatedSKill.SetFollowingFlag(isFollowing);
        return resume;
    }

    private async Task<Resume> MapSkillToResumeStepDefaultChecks(
        Guid currentUserId,
        Guid skillId,
        bool isFollowing)
    {
        var resume = await FindResumeByUserAsync(currentUserId);
        var skill = await _skillRepository.FindAsync(skillId);
        if (skill is null)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.SkillNotFound);
        }
        if (resume.ResumeSkills.All(e => e.SkillId != skillId))
        {
            resume = AddSkillToResume(resume, skill, isFollowing);
        }

        return resume;
    }

    private Resume AddSkillToResume(Resume resume, Skill skill, bool isFollowingSkill)
    {
        var newResumeSkill = new ResumeSkill(
            resume.Id,
            skill.Id,
            isFollowingSkill);

        resume.AddResumeSkill(newResumeSkill);
        return resume;
    }

    private async Task<Resume> FindResumeByUserAsync(Guid userId)
    {
        var resume = await _resumeRepository.FindAsync(e => e.UserId == userId, includeDetails: true);
        if (resume is null)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.ResumeNotFound)
                .WithData("UserId", userId);
        }

        return resume;
    }
}
