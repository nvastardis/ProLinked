using ProLinked.Domain.Contracts.Resumes;
using ProLinked.Domain.Entities.Resumes;
using ProLinked.Domain.Repositories.Resumes;
using ProLinked.Domain.Shared;
using ProLinked.Domain.Shared.Exceptions;
using ProLinked.Domain.Shared.Resumes;

namespace ProLinked.Domain.Services;

public class ResumeManager: IResumeManager
{
    private readonly ISkillRepository _skillRepository;
    private readonly IResumeRepository _resumeRepository;
    private readonly IRepository<ResumeSkill> _resumeSkillRepository;
    private readonly IRepository<EducationStep,Guid> _educationStepRepository;
    private readonly IRepository<EducationStepSkill> _educationStepSkillRepository;
    private readonly IRepository<ExperienceStep,Guid> _experienceStepRepository;
    private readonly IRepository<ExperienceStepSkill> _experienceStepSkillRepository;

    public ResumeManager(
        ISkillRepository skillRepository,
        IResumeRepository resumeRepository,
        IRepository<ResumeSkill> resumeSkillRepository,
        IRepository<EducationStep, Guid> educationStepRepository,
        IRepository<EducationStepSkill> educationStepSkillRepository,
        IRepository<ExperienceStep, Guid> experienceStepRepository,
        IRepository<ExperienceStepSkill> experienceStepSkillRepository)
    {
        _skillRepository = skillRepository;
        _resumeRepository = resumeRepository;
        _resumeSkillRepository = resumeSkillRepository;
        _educationStepRepository = educationStepRepository;
        _educationStepSkillRepository = educationStepSkillRepository;
        _experienceStepRepository = experienceStepRepository;
        _experienceStepSkillRepository = experienceStepSkillRepository;
    }

    public async Task<Resume> CreateResumeAsync(
        Guid currentUserId,
        CancellationToken cancellationToken = default)
    {
        var resume = await _resumeRepository.FindAsync(e => e.UserId == currentUserId, false, cancellationToken);
        if (resume is not null)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.ResumeAlreadyExists);
        }

        cancellationToken.ThrowIfCancellationRequested();
        var newResume = new Resume(
            Guid.NewGuid(),
            currentUserId);
        await _resumeRepository.InsertAsync(newResume, autoSave: true, cancellationToken);

        return newResume;
    }

    public async Task<Skill> CreateSkillAsync(
        string title,
        CancellationToken cancellationToken = default)
    {
        var skillFound = await _skillRepository.FindByTitleAsync(title, cancellationToken);
        if (skillFound is not null)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.SkillAlreadyExists)
                .WithData("Existing skillId", skillFound.Id);
        }

        cancellationToken.ThrowIfCancellationRequested();
        var newSkill = new Skill(
            Guid.NewGuid(),
            title);
        await _skillRepository.InsertAsync(newSkill, autoSave: true, cancellationToken);
        return newSkill;
    }

    public async Task<EducationStep> AddEducationStepAsync(
        Resume resume,
        string school,
        string? degree = null,
        string? fieldOfStudy = null,
        string? grade = null,
        string? activitiesAndSocieties = null,
        string? description = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
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

        cancellationToken.ThrowIfCancellationRequested();
        resume.AddEducationStep(newEducationStep);

        await _educationStepRepository.InsertAsync(newEducationStep, autoSave: true, cancellationToken);
        return newEducationStep;
    }

    public async Task UpdateEducationStepAsync(
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
        CancellationToken cancellationToken = default)
    {
        var step = resume.Education.FirstOrDefault(e => e.Id == educationStepId);
        if (step is null)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.EducationStepNotFound);
        }

        cancellationToken.ThrowIfCancellationRequested();
        step.UpdateEducation(
            school,
            degree,
            fieldOfStudy,
            grade,
            activitiesAndSocieties,
            description,
            startDate,
            endDate);
        await _educationStepRepository.UpdateAsync(step, autoSave: true, cancellationToken);
    }

    public async Task<ExperienceStep> AddExperienceStepAsync(
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
        CancellationToken cancellationToken = default)
    {
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

        cancellationToken.ThrowIfCancellationRequested();
        resume.AddExperienceStep(newExperience);
        await _experienceStepRepository.InsertAsync(newExperience, autoSave: true, cancellationToken);
        return newExperience;
    }

    public async Task UpdateExperienceStepAsync(
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
        CancellationToken cancellationToken = default)
    {
        var step = resume.Experience.FirstOrDefault(e => e.Id == experienceStepId);
        if (step is null)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.ExperienceStepNotFound);
        }

        cancellationToken.ThrowIfCancellationRequested();
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
        await _experienceStepRepository.UpdateAsync(step, autoSave: true, cancellationToken);

    }

    public async Task<ResumeSkill> MapSkillToResumeAsync(
        Resume resume,
        Guid skillId,
        bool isFollowingSkill,
        CancellationToken cancellationToken = default)
    {
        var skill = await _skillRepository.FindAsync(skillId, false, cancellationToken);
        if (skill is null)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.SkillNotFound);
        }
        if (resume.ResumeSkills.Any(e => e.SkillId == skillId))
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.SkillAlreadyMappedToResume);
        }

        cancellationToken.ThrowIfCancellationRequested();
        return await AddSkillToResume(resume, skill, isFollowingSkill, cancellationToken);
    }

    public async Task<EducationStepSkill> MapSkillToEducationAsync(
        EducationStep education,
        Guid skillId,
        bool isFollowingSkill,
        CancellationToken cancellationToken = default)
    {
        await MapSkillToResumeStepDefaultChecks(
            education.ResumeId,
            skillId, isFollowingSkill,
            cancellationToken);

        if (education.RelatedSkills.Any(e => e.SkillId == skillId))
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.SKillAlreadyMappedToEducationStep);
        }

        var newEducationSkill = new EducationStepSkill(education.Id, skillId);

        cancellationToken.ThrowIfCancellationRequested();
        education.AddRelatedSkill(newEducationSkill);
        await _educationStepSkillRepository.InsertAsync(newEducationSkill, autoSave: true, cancellationToken);

        return newEducationSkill;
    }

    public async Task<ExperienceStepSkill> MapSkillToExperienceAsync(
        ExperienceStep experience,
        Guid skillId,
        bool isFollowingSkill,
        CancellationToken cancellationToken = default)
    {
        await MapSkillToResumeStepDefaultChecks(
            experience.ResumeId,
            skillId, isFollowingSkill,
            cancellationToken);

        if (experience.RelatedSkills.Any(e => e.SkillId == skillId))
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.SKillAlreadyMappedToExperienceStep);
        }
        var newExperienceSkill = new ExperienceStepSkill(experience.Id, skillId);

        cancellationToken.ThrowIfCancellationRequested();
        experience.AddRelatedSkill(newExperienceSkill);
        await _experienceStepSkillRepository.InsertAsync(newExperienceSkill, autoSave: true, cancellationToken);

        return newExperienceSkill;
    }

    public async Task SetFollowingFlagOnSkillAsync(
        ResumeSkill relatedSkill,
        bool isFollowing,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        relatedSkill.SetFollowingFlag(isFollowing);
        await _resumeSkillRepository.UpdateAsync(relatedSkill, autoSave: true, cancellationToken);
    }

    public async Task<Resume> GetResumeAsync(
        Guid resumeId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var resume = await _resumeRepository.FindAsync(resumeId, includeDetails: true, cancellationToken);
        if (resume is null)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.ResumeNotFound)
                .WithData("ResumeId", resumeId);
        }

        if (resume.UserId != userId)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.ResumeNotFound)
                .WithData("UserId", userId);
        }

        return resume;
    }

    public async Task DeleteResumeAsync(Resume resume, CancellationToken cancellationToken = default)
    {
        await _resumeRepository.DeleteAsync(resume, autoSave: true, cancellationToken);
    }

    public async Task<ResumeSkill> GetResumeSkillAsync(
        Guid resumeId,
        Guid skillId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var resume = await GetResumeAsync(resumeId, userId, cancellationToken);
        var relatedSKill = resume.ResumeSkills.FirstOrDefault(e => e.SkillId == skillId);
        if (relatedSKill is null)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.SkillNotMappedInResume);
        }

        return relatedSKill;
    }

    public async Task DeleteResumeSkillAsync(
        ResumeSkill resumeSkill,
        CancellationToken cancellationToken = default)
    {
        await _resumeSkillRepository.DeleteAsync(resumeSkill, autoSave: true, cancellationToken);
    }

    public async Task<EducationStep> GetEducationStepAsync(
        Guid resumeId,
        Guid stepId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var resume = await GetResumeAsync(resumeId, userId, cancellationToken);
        var education = resume.Education.FirstOrDefault(e => e.Id == stepId);
        if (education is null)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.EducationStepNotFound)
                .WithData("EducationStepId", stepId);
        }

        return education;
    }

    public async Task DeleteEducationStepAsync(
        EducationStep educationStep,
        CancellationToken cancellationToken = default)
    {
        await _educationStepRepository.DeleteAsync(educationStep, autoSave: true, cancellationToken);
    }

    public async Task DeleteEducationStepSkillAsync(
        EducationStep educationStep,
        Guid skillId,
        CancellationToken cancellationToken = default)
    {
        var relation = await _educationStepSkillRepository.GetAsync(
            e => e.EducationStepId == educationStep.Id && e.SkillId == skillId,
            includeDetails: false,
            cancellationToken);

        await _educationStepSkillRepository.DeleteAsync(relation, autoSave: true, cancellationToken);
    }

    public async Task<ExperienceStep> GetExperienceStepAsync(
        Guid resumeId,
        Guid stepId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var resume = await GetResumeAsync(resumeId, userId, cancellationToken);
        var experience = resume.Experience.FirstOrDefault(e => e.Id == stepId);
        if (experience is null)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.EducationStepNotFound)
                .WithData("EducationStepId", stepId);
        }

        return experience;
    }

    public async Task DeleteExperienceStepAsync(
        ExperienceStep experienceStep,
        CancellationToken cancellationToken = default)
    {
        await _experienceStepRepository.DeleteAsync(experienceStep, autoSave: true, cancellationToken);
    }

    public async Task DeleteExperienceStepSkillAsync(
        ExperienceStep experienceStep,
        Guid skillId,
        CancellationToken cancellationToken = default)
    {
        var relation = await _experienceStepSkillRepository.GetAsync(
            e => e.ExperienceStepId == experienceStep.Id && e.SkillId == skillId,
            includeDetails: false,
            cancellationToken);

        await _experienceStepSkillRepository.DeleteAsync(relation, autoSave: true, cancellationToken);
    }

    private async Task MapSkillToResumeStepDefaultChecks(
        Guid resumeId,
        Guid skillId,
        bool isFollowing,
        CancellationToken cancellationToken = default)
    {
        var skill = await _skillRepository.GetAsync(skillId, includeDetails: true, cancellationToken);
        var resume = await _resumeRepository.GetAsync(resumeId, includeDetails: true, cancellationToken);

        if (resume.ResumeSkills.All(e => e.SkillId != skillId))
        {
           await AddSkillToResume(resume, skill, isFollowing, cancellationToken);
        }
    }

    private async Task<ResumeSkill> AddSkillToResume(
        Resume resume,
        Skill skill,
        bool isFollowingSkill,
        CancellationToken cancellationToken = default)
    {
        var newResumeSkill = new ResumeSkill(
            resume.Id,
            skill.Id,
            isFollowingSkill);

        resume.AddResumeSkill(newResumeSkill);
        await _resumeSkillRepository.InsertAsync(newResumeSkill, autoSave: true, cancellationToken);

        return newResumeSkill;
    }
}