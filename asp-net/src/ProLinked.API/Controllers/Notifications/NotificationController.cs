using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProLinked.Application.Contracts.Notifications;
using ProLinked.Application.Contracts.Notifications.DTOs;
using ProLinked.Application.DTOs;
using ProLinked.Domain;
using ProLinked.Domain.Shared.Identity;

namespace ProLinked.API.Controllers.Notifications;

[ApiController]
[Route("api/notification")]
[Authorize(Roles=RoleConsts.UserRoleName)]
public class NotificationController: ProLinkedController
{
    private readonly INotificationService _notificationService;

    public NotificationController(
        ILogger logger,
        INotificationService notificationService)
        : base(logger)
    {
        _notificationService = notificationService;
    }

    [HttpGet]
    [Route("list")]
    public async Task<Results<Ok<PagedAndSortedResultList<NotificationLookUpDto>>, BadRequest<string>, ProblemHttpResult>>
        GetNotificationList(
            [FromQuery] int? skipCount,
            [FromQuery] int? maxResultCount,
            CancellationToken cancellationToken = default)
    {
        return await OkWithStandardExceptionHandling(
            _notificationService.GetNotificationList(
                GetCurrentUserId(),
                skipCount ?? ProLinkedConsts.SkipCountDefaultValue,
                maxResultCount ?? ProLinkedConsts.MaxResultCountDefaultValue,
                cancellationToken)
        );
    }

    [HttpPut]
    [Route("{id}/update-status")]
    public async Task<Results<NoContent, BadRequest<string>, ProblemHttpResult>> UpdateShownStatusAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await NoContentWithStandardExceptionHandling(
            _notificationService.UpdateShownStatusAsync(
                GetCurrentUserId(),
                id,
                cancellationToken)
        );
    }
}