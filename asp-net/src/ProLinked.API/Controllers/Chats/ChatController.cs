using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProLinked.Application.Contracts.Chats;
using ProLinked.Application.DTOs;
using ProLinked.Application.DTOs.Chats;
using ProLinked.Application.DTOs.Filtering;
using ProLinked.Domain;
using System.ComponentModel.DataAnnotations;

namespace ProLinked.API.Controllers.Chats;

[ApiController]
[Route("api/chat")]
public class ChatController: ProLinkedController
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpGet]
    [Route("list")]
    public async Task<Results<Ok<PagedAndSortedResultList<ChatLookUpDto>>, ProblemHttpResult>> GetListLookUpAsync(
        [FromQuery] string? sorting,
        [FromQuery] int? skipCount,
        [FromQuery] int? maxResultCount,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await OkWithStandardExceptionHandling(
            _chatService.GetListLookUpAsync(
                new ListFilterDto()
                {
                    Sorting = sorting,
                    SkipCount = skipCount ?? ProLinkedConsts.SkipCountDefaultValue,
                    MaxResultCount = maxResultCount ?? ProLinkedConsts.MaxResultCountDefaultValue
                },
                userId,
                cancellationToken
            )
        );
    }

    [HttpGet]
    [Route("{id}/messages")]
    public async Task<Results<Ok<PagedAndSortedResultList<MessageLookUpDto>>, ProblemHttpResult>> GetMessageListAsync(
        [Required] Guid id,
        [FromQuery] string? sorting,
        [FromQuery] int? skipCount,
        [FromQuery] int? maxResultCount,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await OkWithStandardExceptionHandling(
            _chatService.GetMessageListAsync(
                new ChatListFilterDto()
                {
                    ChatId = id,
                    Sorting = sorting,
                    SkipCount = skipCount ?? ProLinkedConsts.SkipCountDefaultValue,
                    MaxResultCount = maxResultCount ?? ProLinkedConsts.MaxResultCountDefaultValue
                },
                userId,
                cancellationToken
            )
        );
    }

    [HttpGet]
    [Route("{id}/members")]
    public async Task<Results<Ok<PagedAndSortedResultList<ChatMembershipLookUpDto>>, ProblemHttpResult>> GetMemberListAsync(
        [Required] Guid id,
        [FromQuery] string? sorting,
        [FromQuery] int? skipCount,
        [FromQuery] int? maxResultCount,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await OkWithStandardExceptionHandling(_chatService.GetMemberListAsync(
            new ChatListFilterDto()
            {
                ChatId = id,
                Sorting = sorting,
                SkipCount = skipCount ?? ProLinkedConsts.SkipCountDefaultValue,
                MaxResultCount = maxResultCount ?? ProLinkedConsts.MaxResultCountDefaultValue,
            },
            userId,
            cancellationToken)
        );
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<Results<Ok<ChatWithDetailsDto>,ProblemHttpResult>> GetDetailsAsync(
        [Required] Guid id,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await OkWithStandardExceptionHandling(
            _chatService.GetDetailsAsync(
                id,
                userId,
                cancellationToken
            )
        );
    }

    [HttpPost]
    [Route("{id}/add-message")]
    public async Task<Results<NoContent, ProblemHttpResult>> AddMessageByChatAsync(
        [Required] Guid id,
        [Required, FromBody] MessageCreateDto input,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await NoContentWithStandardExceptionHandling(
            _chatService.AddMessageByChatAsync(
                id,
                input,
                userId,
                cancellationToken
            )
        );
    }

    [HttpPost]
    [Route("send-message-to-user/{targetUserId}")]
    public async Task<Results<NoContent,ProblemHttpResult>> AddMessageByUserAsync(
        [Required] Guid targetUserId,
        [Required, FromBody] MessageCreateDto input,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await NoContentWithStandardExceptionHandling(
            _chatService.AddMessageByUserAsync(
                targetUserId,
                input,
                userId,
                cancellationToken
            )
        );
    }

    [HttpPost]
    [Route("{id}/add-member")]
    public async Task<Results<NoContent, ProblemHttpResult>> AddMemberAsync(
        [Required] Guid id,
        [Required, FromBody] MemberCreateDto input,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await NoContentWithStandardExceptionHandling(
            _chatService.AddMemberAsync(
                id,
                input,
                userId,
                cancellationToken
            )
        );
    }

    [HttpDelete]
    [Route("{id}/delete-member/{memberId}")]
    public async Task<Results<NoContent, ProblemHttpResult>> DeleteMemberAsync(
        [Required] Guid id,
        [Required] Guid memberId,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await NoContentWithStandardExceptionHandling(
            _chatService.DeleteMemberAsync(
                id,
                userId,
                memberId,
                cancellationToken
            )
        );
    }

    [HttpPost]
    public async Task<Results<NoContent, ProblemHttpResult>> CreateAsync(
        [Required, FromBody] ChatCreateDto input,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await NoContentWithStandardExceptionHandling(
            _chatService.CreateAsync(
                input,
                userId,
                cancellationToken
            )
        );
    }

    [HttpPut]
    [Route("{id}/update-title")]
    public async Task<Results<NoContent, ProblemHttpResult>> UpdateTitleAsync(
        [Required] Guid id,
        [Required] ChatUpdateTitleDto input,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await NoContentWithStandardExceptionHandling(
            _chatService.UpdateTitleAsync(
                id,
                input,
                userId,
                cancellationToken
            )
        );
    }

    [HttpPut]
    [Route("{id}/update-image")]
    public async Task<Results<NoContent, ProblemHttpResult>> UpdateImageAsync(
        [Required] Guid id,
        [Required] ChatUpdateImageDto input,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await NoContentWithStandardExceptionHandling(
            _chatService.UpdateImageAsync(
                id,
                input,
                userId,
                cancellationToken
            )
        );
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<Results<NoContent, ProblemHttpResult>> DeleteAsync(
        [Required] Guid id,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await NoContentWithStandardExceptionHandling(
            _chatService.DeleteAsync(
                id,
                userId,
                cancellationToken
            )
        );
    }
}