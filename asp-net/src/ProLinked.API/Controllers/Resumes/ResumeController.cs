using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProLinked.Application.Contracts.Resumes;
using ProLinked.Application.Contracts.Resumes.DTOs;
using ProLinked.Application.DTOs;

namespace ProLinked.API.Controllers.Resumes;

[ApiController]
[Route("api/resume")]
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
    public async Task<Results<Ok<ResumeDto>, ProblemHttpResult>> GetResumeAsync(
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
    public async Task<Results<Ok<PagedAndSortedResultList<SkillDto>>, ProblemHttpResult>> GetResumeSkillsAsync(
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
    public async Task<Results<NoContent, ProblemHttpResult>> CreateResumeAsync(
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await NoContentWithStandardExceptionHandling(
            _resumeService.CreateResumeAsync(
                userId,
                cancellationToken)
        );
    }

    [HttpPost]
    [Route("{id}/skill/create")]
    public async Task<Results<NoContent, ProblemHttpResult>> CreateResumeSkillAsync(
        Guid id,
        Guid skillId,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await NoContentWithStandardExceptionHandling(
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
    public async Task<Results<NoContent, ProblemHttpResult>> DeleteResumeSkillAsync(
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
    public async Task<Results<Ok<PagedAndSortedResultList<EducationStepDto>>, ProblemHttpResult>> GetEducationStepsAsync(
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
    public async Task<Results<NoContent,ProblemHttpResult>> CreateEducationStepAsync(
        EducationStepCUDto input,
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await NoContentWithStandardExceptionHandling(
            _resumeService.CreateEducationStepAsync(
                input,
                id,
                userId,
                cancellationToken)
        );
    }

    [HttpPut]
    [Route("{id}/step/education/{stepId}/update")]
    public async Task<Results<NoContent,ProblemHttpResult>> UpdateEducationStepAsync(
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
    public async Task<Results<NoContent,ProblemHttpResult>> MapSkillToEducationStepAsync(
        Guid id,
        Guid stepId,
        [FromBody] Guid skillId,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await NoContentWithStandardExceptionHandling(
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
    public async Task<Results<NoContent,ProblemHttpResult>> DeleteSkillFromEducationStepAsync(
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
    public async Task<Results<NoContent,ProblemHttpResult>> DeleteEducationStepAsync(
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
    public async Task<Results<Ok<PagedAndSortedResultList<ExperienceStepDto>>, ProblemHttpResult>> GetExperienceStepsAsync(
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
    public async Task<Results<NoContent,ProblemHttpResult>> CreateExperienceStepAsync(
        ExperienceStepCUDto input,
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await NoContentWithStandardExceptionHandling(
            _resumeService.CreateExperienceStepAsync(
                input,
                id,
                userId,
                cancellationToken)
        );
    }

    [HttpPut]
    [Route("{id}/step/experience/{stepId}/update")]
    public async Task<Results<NoContent,ProblemHttpResult>> UpdateExperienceStepAsync(
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
    public async Task<Results<NoContent,ProblemHttpResult>> MapSkillToExperienceStepAsync(
        Guid id,
        Guid stepId,
        [FromBody] Guid skillId,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await NoContentWithStandardExceptionHandling(
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
    public async Task<Results<NoContent,ProblemHttpResult>> DeleteSkillFromExperienceStepAsync(
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
    public async Task<Results<NoContent,ProblemHttpResult>> DeleteExperienceStepAsync(
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