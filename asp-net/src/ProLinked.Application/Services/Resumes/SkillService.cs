using AutoMapper;
using Microsoft.AspNetCore.Components;
using ProLinked.Application.Contracts.Resumes;
using ProLinked.Application.DTOs;
using ProLinked.Application.DTOs.Resumes;
using ProLinked.Domain.Contracts.Resumes;
using ProLinked.Domain.Entities.Resumes;
using ProLinked.Domain.Repositories.Resumes;

namespace ProLinked.Application.Services.Resumes;

public class SkillService: ProLinkedServiceBase, ISkillService
{
    private readonly IResumeManager ResumeManager;
    private readonly ISkillRepository SkillRepository;

    public SkillService(
        IMapper objectMapper,
        IResumeManager resumeManager,
        ISkillRepository skillRepository)
    : base(objectMapper)
    {
        ResumeManager = resumeManager;
        SkillRepository = skillRepository;
    }

    public async Task<PagedAndSortedResultList<SkillDto>> GetSkillListAsync(
        CancellationToken cancellationToken = default)
    {
        var queryResult = await SkillRepository.GetListAsync(
            includeDetails: false,
            cancellationToken);

        var items = ObjectMapper.Map<List<Skill>, List<SkillDto>>(queryResult);
        var itemCount = items.Count;

        return new PagedAndSortedResultList<SkillDto>(itemCount, items);
    }

    public async Task CreateSkillAsync(
        string title,
        CancellationToken cancellationToken = default)
    {
        await ResumeManager.CreateSkillAsync(
            title,
            cancellationToken);
    }
}