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

namespace ProLinked.API.Controllers.Posts;

[ApiController]
[Route("api/post")]
[Authorize]
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
    public async Task<Results<Ok<PagedAndSortedResultList<PostLookUpDto>>, BadRequest<string>, ProblemHttpResult>> GetPostListAsync(
        [FromQuery] Guid? userId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] PostVisibilityEnum? visibilityEnum,
        [FromQuery] string? sorting,
        [FromQuery] int? skipCount,
        [FromQuery] int? maxResultCount,
        CancellationToken cancellationToken = default)
    {
        var currentUserId = GetCurrentUserId();
        return await OkWithStandardExceptionHandling(
            _postService.GetPostListAsync(
                new PostListFilterDto
                {
                    UserId = userId,
                    VisibilityEnum = userId == currentUserId ? PostVisibilityEnum.UNDEFINED : PostVisibilityEnum.PUBLIC,
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
    public async Task<Results<Ok<PostWithDetailsDto>, BadRequest<string>, ProblemHttpResult>> GetWithDetailsAsync(
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
    [Authorize(Roles=RoleConsts.UserRoleName)]
    public async Task<Results<Created, BadRequest<string>, ProblemHttpResult>> CreatePostAsync(
        PostCUDto input,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await CreatedWithStandardExceptionHandling(
            _postService.CreatePostAsync(
                input,
                userId,
                cancellationToken)
        );
    }

    [HttpPut]
    [Route("{id}/update")]
    [Authorize(Roles=RoleConsts.UserRoleName)]
    public async Task<Results<NoContent, BadRequest<string>, ProblemHttpResult>> UpdatePostAsync(
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
    [Authorize(Roles=RoleConsts.UserRoleName)]
    public async Task<Results<NoContent, BadRequest<string>, ProblemHttpResult>> SetVisibilityAsync(
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
    [Authorize(Roles=RoleConsts.UserRoleName)]
    public async Task<Results<NoContent, BadRequest<string>, ProblemHttpResult>> DeletePostAsync(
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
    public async Task<Results<Ok<PagedAndSortedResultList<ReactionDto>>, BadRequest<string>, ProblemHttpResult>> GetPostReactionListAsync(
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
    [Authorize(Roles=RoleConsts.UserRoleName)]
    public async Task<Results<Created, BadRequest<string>, ProblemHttpResult>> CreatePostReactionAsync(
        Guid postId,
        ReactionTypeEnum reactionType,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await CreatedWithStandardExceptionHandling(
            _postService.CreatePostReactionAsync(
                postId,
                userId,
                reactionType,
                cancellationToken)
        );
    }

    [HttpDelete]
    [Route("{postId}/reaction/{id}")]
    [Authorize(Roles=RoleConsts.UserRoleName)]
    public async Task<Results<NoContent, BadRequest<string>, ProblemHttpResult>> DeletePostReactionAsync(
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