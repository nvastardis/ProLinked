using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProLinked.Application.Contracts.Posts;
using ProLinked.Application.DTOs;
using ProLinked.Application.DTOs.Filtering;
using ProLinked.Application.DTOs.Posts;
using ProLinked.Domain;
using ProLinked.Domain.Shared.Posts;

namespace ProLinked.API.Controllers.Posts;

[ApiController]
[Route("api/post")]
public class PostController: ProLinkedController
{
    private readonly IPostService _postService;

    public PostController(
        ILogger<PostController> logger,
        IPostService postService)
        : base(logger)
    {
        _postService = postService;
    }

    /* Posts */
    [HttpGet]
    [Route("list")]
    public async Task<Results<Ok<PagedAndSortedResultList<PostLookUpDto>>, ProblemHttpResult>> GetPostListAsync(
        [FromQuery] Guid? userId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] PostVisibilityEnum? visibilityEnum,
        [FromQuery] string? sorting,
        [FromQuery] int? skipCount,
        [FromQuery] int? maxResultCount,
        CancellationToken cancellationToken = default)
    {
        return await OkWithStandardExceptionHandling(
            _postService.GetPostListAsync(
                new PostListFilterDto
                {
                    UserId = userId,
                    VisibilityEnum = visibilityEnum ?? PostVisibilityEnum.UNDEFINED,
                    From = from,
                    To = to,
                    IncludeDetails = false,
                    SkipCount = skipCount ?? ProLinkedConsts.SkipCountDefaultValue,
                    MaxResultCount = maxResultCount ?? ProLinkedConsts.MaxResultCountDefaultValue,
                    Sorting = sorting
                },
                cancellationToken)
        );
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<Results<Ok<PostWithDetailsDto>, ProblemHttpResult>> GetWithDetailsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await OkWithStandardExceptionHandling(
            _postService.GetWithDetailsAsync(
                id,
                cancellationToken)
        );
    }

    [HttpPost]
    public async Task<Results<NoContent, ProblemHttpResult>> CreatePostAsync(
        PostCUDto input,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await NoContentWithStandardExceptionHandling(
            _postService.CreatePostAsync(
                input,
                userId,
                cancellationToken)
        );
    }

    [HttpPut]
    [Route("{id}/update")]
    public async Task<Results<NoContent, ProblemHttpResult>> UpdatePostAsync(
        PostCUDto input,
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await NoContentWithStandardExceptionHandling(
            _postService.UpdatePostAsync(
                input,
                id,
                userId,
                cancellationToken)
        );
    }

    [HttpPut]
    [Route("{id}/set-visibility")]
    public async Task<Results<NoContent, ProblemHttpResult>> SetVisibilityAsync(
        Guid id,
        PostVisibilityEnum visibilityEnum,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await NoContentWithStandardExceptionHandling(
            _postService.SetVisibilityAsync(
                id,
                userId,
                visibilityEnum,
                cancellationToken)
        );
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<Results<NoContent, ProblemHttpResult>> DeletePostAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await NoContentWithStandardExceptionHandling(
            _postService.DeletePostAsync(
                id,
                userId,
                cancellationToken)
        );
    }

    /* Post Reactions */
    [HttpGet]
    [Route("{postId}/reaction/list")]
    public async Task<Results<Ok<PagedAndSortedResultList<ReactionDto>>, ProblemHttpResult>> GetPostReactionListAsync(
        Guid postId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] string? sorting,
        [FromQuery] int? skipCount,
        [FromQuery] int? maxResultCount,
        CancellationToken cancellationToken = default)
    {
        return await OkWithStandardExceptionHandling(
            _postService.GetPostReactionListAsync(
                postId,
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
    [Route("{postId}/reaction")]
    public async Task<Results<NoContent, ProblemHttpResult>> CreatePostReactionAsync(
        Guid postId,
        ReactionTypeEnum reactionType,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await NoContentWithStandardExceptionHandling(
            _postService.CreatePostReactionAsync(
                postId,
                userId,
                reactionType,
                cancellationToken)
        );
    }

    [HttpDelete]
    [Route("{postId}/reaction/{id}")]
    public async Task<Results<NoContent, ProblemHttpResult>> DeletePostReactionAsync(
        Guid postId,
        Guid reactionId,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await NoContentWithStandardExceptionHandling(
            _postService.DeletePostReactionAsync(
                postId,
                reactionId,
                userId,
                cancellationToken)
        );
    }

}