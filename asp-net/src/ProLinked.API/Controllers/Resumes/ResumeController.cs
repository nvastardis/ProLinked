using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProLinked.Application.Contracts.Resumes;
using ProLinked.Application.Contracts.Resumes.DTOs;
using ProLinked.Application.DTOs;
using ProLinked.Domain.Shared.Identity;

namespace ProLinked.API.Controllers.Resumes;

[ApiController]
[Route("api/resume")]
[Authorize]
public class ResumeController: ProLinkedController
{
    private readonly IResumeService _resumeService;
    public ResumeController(
        ILogger logger,
        IResumeService resumeService)
        : base(logger)
    {
        _resumeService = resumeService;
    }

    /* Resume */
    [HttpGet]
    [Route("{id}")]
    public async Task<Results<Ok<ResumeDto>, BadRequest<string>, ProblemHttpResult>> GetResumeAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await OkWithStandardExceptionHandling(
            _resumeService.GetResumeAsync(
                id,
                cancellationToken)
        );
    }

    [HttpGet]
    [Route("{id}/skill/list")]
    public async Task<Results<Ok<PagedAndSortedResultList<SkillDto>>, BadRequest<string>, ProblemHttpResult>> GetResumeSkillsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await OkWithStandardExceptionHandling(
            _resumeService.GetResumeSkillsAsync(
                id,
                cancellationToken)
        );
    }

    [HttpPost]
    [Route("create")]
    [Authorize(Roles=RoleConsts.UserRoleName)]
    public async Task<Results<Created, BadRequest<string>, ProblemHttpResult>> CreateResumeAsync(
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await CreatedWithStandardExceptionHandling(
            _resumeService.CreateResumeAsync(
                userId,
                cancellationToken)
        );
    }

    [HttpPost]
    [Route("{id}/skill/create")]
    [Authorize(Roles=RoleConsts.UserRoleName)]
    public async Task<Results<Created, BadRequest<string>, ProblemHttpResult>> CreateResumeSkillAsync(
        Guid id,
        Guid skillId,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await CreatedWithStandardExceptionHandling(
            _resumeService.CreateResumeSkillAsync(
                new SkillToResumeDto
                {
                    ResumeId = id,
                    SkillId = skillId
                },
                userId,
                cancellationToken)
        );
    }

    [HttpDelete]
    [Route("{id}/skill/{skillId}/delete")]
    [Authorize(Roles=RoleConsts.UserRoleName)]
    public async Task<Results<NoContent,BadRequest<string>,  ProblemHttpResult>> DeleteResumeSkillAsync(
        Guid id,
        Guid skillId,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await NoContentWithStandardExceptionHandling(
            _resumeService.DeleteResumeSkillAsync(
                new SkillToResumeDto
                {
                    ResumeId = id,
                    SkillId = skillId
                },
                userId,
                cancellationToken)
            );
    }

    /* Education */
    [HttpGet]
    [Route("{id}/step/education/list")]
    public async Task<Results<Ok<PagedAndSortedResultList<EducationStepDto>>, BadRequest<string>, ProblemHttpResult>> GetEducationStepsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await OkWithStandardExceptionHandling(
            _resumeService.GetEducationStepsAsync(
                id,
                cancellationToken)
        );
    }

    [HttpPost]
    [Route("{id}/step/education/create")]
    [Authorize(Roles=RoleConsts.UserRoleName)]
    public async Task<Results<Created, BadRequest<string>, ProblemHttpResult>> CreateEducationStepAsync(
        EducationStepCUDto input,
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await CreatedWithStandardExceptionHandling(
            _resumeService.CreateEducationStepAsync(
                input,
                id,
                userId,
                cancellationToken)
        );
    }

    [HttpPut]
    [Route("{id}/step/education/{stepId}/update")]
    [Authorize(Roles=RoleConsts.UserRoleName)]
    public async Task<Results<NoContent, BadRequest<string>, ProblemHttpResult>> UpdateEducationStepAsync(
        EducationStepCUDto input,
        Guid id,
        Guid stepId,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await NoContentWithStandardExceptionHandling(
            _resumeService.UpdateEducationStepAsync(
                input,
                stepId,
                id,
                userId,
                cancellationToken)
        );
    }

    [HttpPost]
    [Route("{id}/step/education/{stepId}/skill/create")]
    public async Task<Results<Created, BadRequest<string>, ProblemHttpResult>> MapSkillToEducationStepAsync(
        Guid id,
        Guid stepId,
        [FromBody] Guid skillId,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await CreatedWithStandardExceptionHandling(
            _resumeService.MapSkillToEducationStepAsync(
                new SkillToStepMapDto
                {
                    SkillId = skillId,
                    StepId = stepId
                },
                id,
                userId,
                cancellationToken)
        );
    }

    [HttpDelete]
    [Route("{id}/step/education/{stepId}/skill/{skillId}/delete")]
    [Authorize(Roles=RoleConsts.UserRoleName)]
    public async Task<Results<NoContent, BadRequest<string>, ProblemHttpResult>> DeleteSkillFromEducationStepAsync(
        Guid id,
        Guid stepId,
        Guid skillId,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await NoContentWithStandardExceptionHandling(
            _resumeService.DeleteSkillFromEducationStepAsync(
                new SkillToStepMapDto
                {
                    SkillId = skillId,
                    StepId = stepId
                },
                id,
                userId,
                cancellationToken)
        );
    }

    [HttpDelete]
    [Route("{id}/step/education/{stepId}/delete")]
    [Authorize(Roles=RoleConsts.UserRoleName)]
    public async Task<Results<NoContent, BadRequest<string>, ProblemHttpResult>> DeleteEducationStepAsync(
        Guid id,
        Guid stepId,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await NoContentWithStandardExceptionHandling(
            _resumeService.DeleteEducationStepAsync(
                stepId,
                id,
                userId,
                cancellationToken)
        );
    }

    /* Experience */
    [HttpGet]
    [Route("{id}/step/experience/list")]
    public async Task<Results<Ok<PagedAndSortedResultList<ExperienceStepDto>>, BadRequest<string>, ProblemHttpResult>> GetExperienceStepsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await OkWithStandardExceptionHandling(
            _resumeService.GetExperienceStepsAsync(
                id,
                cancellationToken)
        );
    }

    [HttpPost]
    [Route("{id}/step/experience/create")]
    [Authorize(Roles=RoleConsts.UserRoleName)]
    public async Task<Results<Created, BadRequest<string>, ProblemHttpResult>> CreateExperienceStepAsync(
        ExperienceStepCUDto input,
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await CreatedWithStandardExceptionHandling(
            _resumeService.CreateExperienceStepAsync(
                input,
                id,
                userId,
                cancellationToken)
        );
    }

    [HttpPut]
    [Route("{id}/step/experience/{stepId}/update")]
    [Authorize(Roles=RoleConsts.UserRoleName)]
    public async Task<Results<NoContent, BadRequest<string>, ProblemHttpResult>> UpdateExperienceStepAsync(
        ExperienceStepCUDto input,
        Guid id,
        Guid stepId,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await NoContentWithStandardExceptionHandling(
            _resumeService.UpdateExperienceStepAsync(
                input,
                stepId,
                id,
                userId,
                cancellationToken)
        );
    }

    [HttpPost]
    [Route("{id}/step/experience/{stepId}/skill/create")]
    [Authorize(Roles=RoleConsts.UserRoleName)]
    public async Task<Results<Created, BadRequest<string>, ProblemHttpResult>> MapSkillToExperienceStepAsync(
        Guid id,
        Guid stepId,
        [FromBody] Guid skillId,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await CreatedWithStandardExceptionHandling(
            _resumeService.MapSkillToExperienceStepAsync(
                new SkillToStepMapDto
                {
                    SkillId = skillId,
                    StepId = stepId
                },
                id,
                userId,
                cancellationToken)
        );
    }

    [HttpDelete]
    [Route("{id}/step/experience/{stepId}/skill/{skillId}/delete")]
    [Authorize(Roles=RoleConsts.UserRoleName)]
    public async Task<Results<NoContent, BadRequest<string>, ProblemHttpResult>> DeleteSkillFromExperienceStepAsync(
        Guid id,
        Guid stepId,
        Guid skillId,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await NoContentWithStandardExceptionHandling(
            _resumeService.DeleteSkillFromExperienceStepAsync(
                new SkillToStepMapDto
                {
                    SkillId = skillId,
                    StepId = stepId
                },
                id,
                userId,
                cancellationToken)
        );
    }

    [HttpDelete]
    [Route("{id}/step/experience/{stepId}/delete")]
    [Authorize(Roles=RoleConsts.UserRoleName)]
    public async Task<Results<NoContent, BadRequest<string>, ProblemHttpResult>> DeleteExperienceStepAsync(
        Guid id,
        Guid stepId,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await NoContentWithStandardExceptionHandling(
            _resumeService.DeleteExperienceStepAsync(
                stepId,
                id,
                userId,
                cancellationToken)
        );
    }
}