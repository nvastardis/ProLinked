using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProLinked.Application.Contracts.Resumes;
using ProLinked.Application.Contracts.Resumes.DTOs;
using ProLinked.Application.DTOs;
using ProLinked.Domain.Shared.Identity;

namespace ProLinked.API.Controllers.Resumes;

[ApiController]
[Route("api/skill")]
[Authorize]
public class SkillController: ProLinkedController
{
    private ISkillService _skillService;
    public SkillController(
        ILogger logger,
        ISkillService skillService)
        : base(logger)
    {
        _skillService = skillService;
    }

    [HttpGet]
    [Route("list")]
    public async Task<Results<Ok<PagedAndSortedResultList<SkillDto>>, BadRequest<string>, ProblemHttpResult>> GetSkillListAsync(
        CancellationToken cancellationToken = default)
    {
        return await OkWithStandardExceptionHandling(
            _skillService.GetSkillListAsync(
                cancellationToken)
        );
    }

    [HttpPost]
    [Route("create")]
    [Authorize(Roles=RoleConsts.UserRoleName)]
    public async Task<Results<Created, BadRequest<string>, ProblemHttpResult>> CreateSkillAsync(
        string title,
        CancellationToken cancellationToken = default)
    {
        return await CreatedWithStandardExceptionHandling(
            _skillService.CreateSkillAsync(
                title,
                cancellationToken)
        );
    }
}