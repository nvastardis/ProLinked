using AutoMapper;
using ProLinked.Application.Contracts.Resumes;
using ProLinked.Application.Contracts.Resumes.DTOs;
using ProLinked.Application.DTOs;
using ProLinked.Domain.Contracts.Resumes;
using ProLinked.Domain.DTOs.Resumes;
using ProLinked.Domain.Entities.Resumes;

namespace ProLinked.Application.Services.Resumes;

public class ResumeService: ProLinkedServiceBase, IResumeService
{
    private readonly IResumeManager ResumeManager;
    private readonly IResumeRepository ResumeRepository;

    public ResumeService(
        IMapper objectMapper,
        IResumeManager resumeManager,
        IResumeRepository resumeRepository)
        : base(objectMapper)
    {
        ResumeManager = resumeManager;
        ResumeRepository = resumeRepository;
    }

    public async Task<ResumeDto> GetResumeAsync(
        Guid resumeId,
        CancellationToken cancellationToken = default)
    {
        var queryResult = await ResumeRepository.GetWithDetailsAsync(
            resumeId,
            cancellationToken);

        var result = ObjectMapper.Map<ResumeWithDetails, ResumeDto>(queryResult);
        return result;
    }

    public async Task<PagedAndSortedResultList<SkillDto>> GetResumeSkillsAsync(
        Guid resumeId,
        CancellationToken cancellationToken = default)
    {
        var queryResult = await ResumeRepository.GetListResumeSkillAsync(
            resumeId,
            cancellationToken);

        var items = ObjectMapper.Map<List<Skill>, List<SkillDto>>(queryResult);
        var itemCount = items.Count;
        return new PagedAndSortedResultList<SkillDto>(itemCount, items);
    }

    public async Task CreateResumeAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        await ResumeManager.CreateResumeAsync(
            userId,
            cancellationToken);
    }

    public async Task CreateResumeSkillAsync(
        SkillToResumeDto input,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var resume = await ResumeManager.GetResumeAsync(
            input.ResumeId,
            userId,
            cancellationToken);

        await ResumeManager.MapSkillToResumeAsync(
            resume,
            input.SkillId,
            input.IsFollowing,
            cancellationToken);
    }

    public async Task UpdateIsFollowingStatusAsync(
        SkillToResumeDto input,
        Guid userId,
        bool isFollowing,
        CancellationToken cancellationToken = default)
    {
        var item = await ResumeManager.GetResumeSkillAsync(
            input.ResumeId,
            input.SkillId,
            userId,
            cancellationToken);

        await ResumeManager.SetFollowingFlagOnSkillAsync(
            item,
            isFollowing,
            cancellationToken);
    }

    public async Task DeleteResumeSkillAsync(
        SkillToResumeDto input,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var itemToDelete = await ResumeManager.GetResumeSkillAsync(
            input.ResumeId,
            input.SkillId,
            userId,
            cancellationToken);

        await ResumeManager.DeleteResumeSkillAsync(itemToDelete, cancellationToken);
    }

    public async Task<PagedAndSortedResultList<EducationStepDto>> GetEducationStepsAsync(
        Guid resumeId,
        CancellationToken cancellationToken = default)
    {
        var queryResult = await ResumeRepository.GetListEducationStepByUserAsync(
            resumeId,
            cancellationToken);

        var items = ObjectMapper.Map<List<EducationStep>, List<EducationStepDto>>(queryResult);
        var itemCount = items.Count;
        return new PagedAndSortedResultList<EducationStepDto>(itemCount, items);
    }

    public async Task CreateEducationStepAsync(
        EducationStepCUDto input,
        Guid resumeId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var resume = await ResumeManager.GetResumeAsync(
            resumeId,
            userId,
            cancellationToken);
        await ResumeManager.AddEducationStepAsync(
            resume,
            input.School,
            input.Degree,
            input.FieldOfStudy,
            input.Grade,
            input.Activities,
            input.Description,
            input.StartDate,
            input.EndDate,
            cancellationToken);
    }

    public async Task UpdateEducationStepAsync(
        EducationStepCUDto input,
        Guid educationStepId,
        Guid resumeId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var resume = await ResumeManager.GetResumeAsync(
            resumeId,
            userId,
            cancellationToken);

        await ResumeManager.UpdateEducationStepAsync(
            resume,
            educationStepId,
            input.School,
            input.Degree,
            input.FieldOfStudy,
            input.Grade,
            input.Activities,
            input.Description,
            input.StartDate,
            input.EndDate,
            cancellationToken);
    }

    public async Task MapSkillToEducationStepAsync(
        SkillToStepMapDto input,
        Guid resumeId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var educationStep = await ResumeManager.GetEducationStepAsync(
            resumeId,
            input.StepId,
            userId,
            cancellationToken);

        await ResumeManager.MapSkillToEducationAsync(
            educationStep,
            input.SkillId,
            input.IsFollowingSkill,
            cancellationToken);
    }

    public async Task DeleteSkillFromEducationStepAsync(
        SkillToStepMapDto input,
        Guid resumeId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var educationStep = await ResumeManager.GetEducationStepAsync(
            resumeId,
            input.StepId,
            userId,
            cancellationToken);

        await ResumeManager.DeleteEducationStepSkillAsync(
            educationStep,
            input.SkillId,
            cancellationToken);
    }

    public async Task DeleteEducationStepAsync(
        Guid educationStepId,
        Guid resumeId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var educationStep = await ResumeManager.GetEducationStepAsync(
            resumeId,
            educationStepId,
            userId,
            cancellationToken);

        await ResumeManager.DeleteEducationStepAsync(educationStep, cancellationToken);
    }

    public async Task<PagedAndSortedResultList<ExperienceStepDto>> GetExperienceStepsAsync(
        Guid resumeId,
        CancellationToken cancellationToken = default)
    {
        var queryResult = await ResumeRepository.GetListExperienceStepByUserAsync(
            resumeId,
            cancellationToken);

        var items = ObjectMapper.Map<List<ExperienceStep>, List<ExperienceStepDto>>(queryResult);
        var itemCount = items.Count;
        return new PagedAndSortedResultList<ExperienceStepDto>(itemCount, items);
    }

    public async Task CreateExperienceStepAsync(
        ExperienceStepCUDto input,
        Guid resumeId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var resume = await ResumeManager.GetResumeAsync(
            resumeId,
            userId,
            cancellationToken);

        await ResumeManager.AddExperienceStepAsync(
            resume,
            input.Title,
            input.Company,
            input.EmploymentType,
            input.IsEmployed,
            input.Location,
            input.WorkArrangement,
            input.Description,
            input.StartDate,
            input.EndDate,
            cancellationToken);
    }

    public async Task UpdateExperienceStepAsync(
        ExperienceStepCUDto input,
        Guid stepId,
        Guid resumeId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var resume = await ResumeManager.GetResumeAsync(
            resumeId,
            userId,
            cancellationToken);

        await ResumeManager.UpdateExperienceStepAsync(
            resume,
            stepId,
            input.Title,
            input.Company,
            input.EmploymentType,
            input.IsEmployed,
            input.Location,
            input.WorkArrangement,
            input.Description,
            input.StartDate,
            input.EndDate,
            cancellationToken);
    }

    public async Task MapSkillToExperienceStepAsync(
        SkillToStepMapDto input,
        Guid resumeId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var experienceStep = await ResumeManager.GetExperienceStepAsync(
            resumeId,
            input.StepId,
            userId,
            cancellationToken);

        await ResumeManager.MapSkillToExperienceAsync(
            experienceStep,
            input.SkillId,
            input.IsFollowingSkill,
            cancellationToken);
    }

    public async Task DeleteSkillFromExperienceStepAsync(
        SkillToStepMapDto input,
        Guid resumeId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var experienceStep = await ResumeManager.GetExperienceStepAsync(
            resumeId,
            input.StepId,
            userId,
            cancellationToken);

        await ResumeManager.DeleteExperienceStepSkillAsync(
            experienceStep,
            input.SkillId,
            cancellationToken);
    }

    public async Task DeleteExperienceStepAsync(
        Guid stepId,
        Guid resumeId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var experienceStep = await ResumeManager.GetEducationStepAsync(
            resumeId,
            stepId,
            userId,
            cancellationToken);

        await ResumeManager.DeleteEducationStepAsync(experienceStep, cancellationToken);
    }
}