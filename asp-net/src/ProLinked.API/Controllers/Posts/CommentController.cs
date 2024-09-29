using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProLinked.Application.Contracts.Filtering;
using ProLinked.Application.Contracts.Posts;
using ProLinked.Application.Contracts.Posts.DTOs;
using ProLinked.Application.DTOs;
using ProLinked.Domain;
using ProLinked.Domain.Shared.Identity;
using ProLinked.Domain.Shared.Posts;
using System.ComponentModel.DataAnnotations;

namespace ProLinked.API.Controllers.Posts;

[ApiController]
[Route("api/comment")]
[Authorize]
public class CommentController: ProLinkedController
{
    private readonly ICommentService _commentService;

    public CommentController(
        ILogger<CommentController> logger,
        ICommentService commentService)
        : base(logger)
    {
        _commentService = commentService;
    }

    [HttpGet]
    [Route("list")]
    public async Task<Results<Ok<PagedAndSortedResultList<CommentDto>>, BadRequest<string>, ProblemHttpResult>> GetCommentListAsync(
        [Required,FromQuery] Guid postId,
        [FromQuery] Guid? userId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] string? sorting,
        [FromQuery] int? skipCount,
        [FromQuery] int? maxResultCount,
        CancellationToken cancellationToken = default)
    {
        return await OkWithStandardExceptionHandling(
            _commentService.GetCommentListForPostAsync(
                new CommentListFilterDto
                {
                    UserId = userId,
                    PostId = postId,
                    From = from,
                    To = to,
                    IncludeDetails = true,
                    Sorting = sorting,
                    SkipCount = skipCount ?? ProLinkedConsts.SkipCountDefaultValue,
                    MaxResultCount = maxResultCount ?? ProLinkedConsts.MaxResultCountDefaultValue
                },
                cancellationToken)
        );
    }

    [HttpPost]
    [Authorize(Roles=RoleConsts.UserRoleName)]
    public async Task<Results<Created, BadRequest<string>, ProblemHttpResult>> CreateCommentAsync(
        CommentCUDto input,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await CreatedWithStandardExceptionHandling(
            _commentService.CreateCommentAsync(
                input,
                userId,
                cancellationToken)
        );
    }

    [HttpPut]
    [Route("{id}/update")]
    [Authorize(Roles=RoleConsts.UserRoleName)]
    public async Task<Results<NoContent, BadRequest<string>, ProblemHttpResult>> UpdateCommentAsync(
        CommentCUDto input,
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await NoContentWithStandardExceptionHandling(
            _commentService.UpdateCommentAsync(
                input,
                id,
                userId,
                cancellationToken)
        );
    }

    [HttpDelete]
    [Route("{id}/delete")]
    [Authorize(Roles=RoleConsts.UserRoleName)]
    public async Task<Results<NoContent, BadRequest<string>, ProblemHttpResult>> DeleteCommentAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await NoContentWithStandardExceptionHandling(
            _commentService.DeleteCommentAsync(
                id,
                userId,
                cancellationToken)
        );
    }

    [HttpGet]
    [Route("{commentId}/reaction/list")]
    public async Task<Results<Ok<PagedAndSortedResultList<ReactionDto>>, BadRequest<string>, ProblemHttpResult>> GetCommentReactionListAsync(
        Guid commentId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] string? sorting,
        [FromQuery] int? skipCount,
        [FromQuery] int? maxResultCount,
        CancellationToken cancellationToken = default)
    {
        return await OkWithStandardExceptionHandling(
            _commentService.GetCommentReactionListAsync(
                commentId,
                new ListFilterDto
                {
                    From = from,
                    To = to,
                    IncludeDetails = true,
                    Sorting = sorting,
                    SkipCount = skipCount ?? ProLinkedConsts.SkipCountDefaultValue,
                    MaxResultCount = maxResultCount ?? ProLinkedConsts.MaxResultCountDefaultValue,
                },
                cancellationToken)
        );
    }

    [HttpPost]
    [Route("{commentId}/reaction")]
    [Authorize(Roles=RoleConsts.UserRoleName)]
    public async Task<Results<Created, BadRequest<string>, ProblemHttpResult>> CreateCommentReactionAsync(
        Guid commentId,
        ReactionTypeEnum reactionType,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await CreatedWithStandardExceptionHandling(
            _commentService.CreateCommentReactionAsync(
                commentId,
                userId,
                reactionType,
                cancellationToken)
        );
    }

    [HttpDelete]
    [Route("{commentId}/reaction/{id}")]
    public async Task<Results<NoContent, BadRequest<string>, ProblemHttpResult>> DeleteCommentReactionAsync(
        Guid commentId,
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await NoContentWithStandardExceptionHandling(
            _commentService.DeleteCommentReactionAsync(
                commentId,
                id,
                userId,
                cancellationToken)
        );
    }
}