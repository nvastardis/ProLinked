using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProLinked.Application.Contracts.Filtering;
using ProLinked.Application.Contracts.Posts;
using ProLinked.Application.Contracts.Posts.DTOs;
using ProLinked.Application.DTOs;
using ProLinked.Domain;
using ProLinked.Domain.Shared.Posts;

namespace ProLinked.API.Controllers.Posts;

[ApiController]
[Route("api/comment")]
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
    public async Task<Results<Ok<PagedAndSortedResultList<CommentDto>>,ProblemHttpResult>> GetCommentListAsync(
        [FromQuery] Guid postId,
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
    public async Task<Results<NoContent, ProblemHttpResult>> CreateCommentAsync(
        CommentCUDto input,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await NoContentWithStandardExceptionHandling(
            _commentService.CreateCommentAsync(
                input,
                userId,
                cancellationToken)
        );
    }

    [HttpPut]
    [Route("{id}/update")]
    public async Task<Results<NoContent, ProblemHttpResult>> UpdateCommentAsync(
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
    public async Task<Results<NoContent, ProblemHttpResult>> DeleteCommentAsync(
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
    public async Task<Results<Ok<PagedAndSortedResultList<ReactionDto>>,ProblemHttpResult>> GetCommentReactionListAsync(
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
    public async Task<Results<NoContent, ProblemHttpResult>> CreateCommentReactionAsync(
        Guid commentId,
        ReactionTypeEnum reactionType,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await NoContentWithStandardExceptionHandling(
            _commentService.CreateCommentReactionAsync(
                commentId,
                userId,
                reactionType,
                cancellationToken)
        );
    }

    [HttpDelete]
    [Route("{commentId}/reaction/{id}")]
    public async Task<Results<NoContent, ProblemHttpResult>> DeleteCommentReactionAsync(
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