using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProLinked.Application.Contracts.Jobs;
using ProLinked.Application.Contracts.Jobs.DTOs;
using ProLinked.Application.DTOs;
using ProLinked.Domain;
using ProLinked.Domain.Shared.Identity;
using ProLinked.Domain.Shared.Jobs;
using System.ComponentModel.DataAnnotations;

namespace ProLinked.API.Controllers.Jobs;

[ApiController]
[Route("api/jobs")]
[Authorize]
public class JobController: ProLinkedController
{
    private readonly IJobService _jobService;

    public JobController(
        ILogger<JobController> logger,
        IJobService jobService)
        : base(logger)
    {
        _jobService = jobService;
    }

    [HttpGet]
    [Route("advertisement/list")]
    public async Task<Results<Ok<PagedAndSortedResultList<AdvertisementDto>>, BadRequest<string>, ProblemHttpResult>>
        GetListOfJobAdvertisementsAsync(
            [FromQuery] Guid? userId,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] AdvertisementStatus? advertisementStatus,
            [FromQuery] string? sorting,
            [FromQuery] int? skipCount,
            [FromQuery] int? maxResultCount,
            CancellationToken cancellationToken = default)
    {
        return await OkWithStandardExceptionHandling(
            _jobService.GetListOfJobAdvertisementsAsync(
                new JobListFilter
                {
                    UserId = userId ?? Guid.Empty,
                    From = from,
                    To = to,
                    AdvertisementStatus = advertisementStatus ?? AdvertisementStatus.UNDEFINED,
                    IncludeDetails = false,
                    Sorting = sorting,
                    SkipCount = skipCount ?? ProLinkedConsts.SkipCountDefaultValue,
                    MaxResultCount = maxResultCount ?? ProLinkedConsts.MaxResultCountDefaultValue,
                },
                cancellationToken)
        );
    }

    [HttpGet]
    [Route("advertisement/{id}/application/list")]
    public async Task<Results<Ok<PagedAndSortedResultList<ApplicationDto>>, BadRequest<string>, ProblemHttpResult>>
        GetListOfApplicationsAsync(
            Guid id,
            CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await OkWithStandardExceptionHandling(
            _jobService.GetListOfApplicationsAsync(
                id,
                userId,
                cancellationToken)
        );
    }

    [HttpPost]
    [Route("advertisement")]
    [Authorize(Roles=RoleConsts.UserRoleName)]
    public async Task<Results<Created, BadRequest<string>, ProblemHttpResult>> CreateJobAdvertisementAsync(
        AdvertisementCUDto input,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await CreatedWithStandardExceptionHandling(
            _jobService.CreateJobAdvertisementAsync(
                input,
                userId,
                cancellationToken)
        );
    }

    [HttpPost]
    [Route("advertisement/{id}/apply")]
    [Authorize(Roles=RoleConsts.UserRoleName)]
    public async Task<Results<Created, BadRequest<string>, ProblemHttpResult>> ApplyAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await CreatedWithStandardExceptionHandling(
            _jobService.ApplyAsync(
                id,
                userId,
                cancellationToken)
        );
    }

    [HttpPut]
    [Route("advertisement/{id}/close")]
    [Authorize(Roles=RoleConsts.UserRoleName)]
    public async Task<Results<NoContent, BadRequest<string>, ProblemHttpResult>> CloseAdvertisement(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await NoContentWithStandardExceptionHandling(
            _jobService.CloseAdvertisement(
                id,
                userId,
                cancellationToken)
        );
    }

    [HttpPut]
    [Route("advertisement/{id}/update")]
    [Authorize(Roles=RoleConsts.UserRoleName)]
    public async Task<Results<NoContent, BadRequest<string>, ProblemHttpResult>> UpdateJobAdvertisementAsync(
        Guid id,
        AdvertisementCUDto input,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await NoContentWithStandardExceptionHandling(
            _jobService.UpdateJobAdvertisementAsync(
                id,
                input,
                userId,
                cancellationToken)
        );
    }

    [HttpGet]
    [Route("application/user")]
    public async Task<Results<Ok<PagedAndSortedResultList<ApplicationDto>>, BadRequest<string>, ProblemHttpResult>>
        GetListOfUserApplicationsAsync(
            [Required, FromQuery] Guid userId,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] ApplicationStatus? applicationStatus,
            [FromQuery] string? sorting,
            [FromQuery] int? skipCount,
            [FromQuery] int? maxResultCount,
            CancellationToken cancellationToken = default)
    {
        return await OkWithStandardExceptionHandling(
            _jobService.GetListOfUserApplicationsAsync(
                new JobListFilter
                {
                    UserId = userId,
                    From = from,
                    To = to,
                    ApplicationStatus = applicationStatus ?? ApplicationStatus.UNDEFINED,
                    IncludeDetails = false,
                    Sorting = sorting,
                    SkipCount = skipCount ?? ProLinkedConsts.SkipCountDefaultValue,
                    MaxResultCount = maxResultCount ?? ProLinkedConsts.MaxResultCountDefaultValue,
                },
                cancellationToken)
        );
    }

    [HttpPut]
    [Route("application/{id}/accept")]
    [Authorize(Roles=RoleConsts.UserRoleName)]
    public async Task<Results<NoContent, BadRequest<string>, ProblemHttpResult>> AcceptApplicationAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await NoContentWithStandardExceptionHandling(
            _jobService.AcceptApplicationAsync(
                id,
                userId,
                cancellationToken)
        );
    }

    [HttpPut]
    [Route("application/{id}/reject")]
    [Authorize(Roles=RoleConsts.UserRoleName)]
    public async Task<Results<NoContent, BadRequest<string>, ProblemHttpResult>> RejectApplicationAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await NoContentWithStandardExceptionHandling(
            _jobService.RejectApplicationAsync(
                id,
                userId,
                cancellationToken)
        );
    }

}