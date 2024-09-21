using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using ProLinked.Application.Contracts.Posts;
using ProLinked.Application.DTOs;
using ProLinked.Application.DTOs.Filtering;
using ProLinked.Application.DTOs.Posts;
using ProLinked.Domain;
using ProLinked.Domain.Contracts.Blobs;
using ProLinked.Domain.Contracts.Posts;
using ProLinked.Domain.Entities.Blobs;
using ProLinked.Domain.Entities.Posts;
using ProLinked.Domain.Extensions;
using ProLinked.Domain.Repositories.Posts;
using ProLinked.Domain.Shared.Posts;

namespace ProLinked.Application.Services.Posts;

public class PostService: ProLinkedServiceBase, IPostService
{
    private readonly IPostManager PostManager;
    private readonly IBlobManager BlobManager;
    private readonly IPostRepository PostRepository;
    private readonly IRepository<Comment, Guid> CommentRepository;

    public PostService(
        IMapper objectMapper,
        ILogger logger,
        IPostManager postManager,
        IPostRepository postRepository,
        IRepository<Comment, Guid> commentRepository,
        IBlobManager blobManager)
        : base(objectMapper, logger)
    {
        PostManager = postManager;
        PostRepository = postRepository;
        CommentRepository = commentRepository;
        BlobManager = blobManager;
    }

    public async Task<PagedAndSortedResultList<PostLookUpDto>> GetPostListAsync(
        PostListFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        var queryResult = await PostRepository.GetListByUserAsync(
            filter.UserId,
            filter.From,
            filter.To,
            filter.IncludeDetails = false,
            filter.Sorting,
            filter.SkipCount,
            filter.MaxResultCount,
            cancellationToken);

        var items = ObjectMapper.Map<List<Post>, List<PostLookUpDto>>(queryResult);
        var itemCount = items.Count;
        return new PagedAndSortedResultList<PostLookUpDto>(itemCount, items);
    }

    public async Task<PagedAndSortedResultList<CommentDto>> GetCommentListAsync(
        Guid postId,
        ListFilterDto filterDto,
        CancellationToken cancellationToken = default)
    {
        var post = await PostRepository.GetAsync(postId, includeDetails: true, cancellationToken);

        var items = ObjectMapper.Map<List<Comment>, List<CommentDto>>(post.Comments.ToList());
        var itemCount = items.Count;
        return new PagedAndSortedResultList<CommentDto>(itemCount, items);
    }

    public async Task<PagedAndSortedResultList<CommentReactionDto>> GetCommentReactionListAsync(
        Guid commentId,
        ListFilterDto filterDto,
        CancellationToken cancellationToken = default)
    {
        var comment = await CommentRepository.GetAsync(commentId, includeDetails: true, cancellationToken);
        var items = ObjectMapper.Map<List<CommentReaction>, List<CommentReactionDto>>(comment.Reactions.ToList());
        var itemCount = items.Count;
        return new PagedAndSortedResultList<CommentReactionDto>(itemCount, items);
    }

    public async Task<PagedAndSortedResultList<PostReactionDto>> GetPostReactionListAsync(
        Guid postId,
        ListFilterDto filterDto,
        CancellationToken cancellationToken = default)
    {
        var post = await PostRepository.GetAsync(postId, includeDetails: true, cancellationToken);
        var items = ObjectMapper.Map<List<PostReaction>, List<PostReactionDto>>(post.Reactions.ToList());
        var itemCount = items.Count;
        return new PagedAndSortedResultList<PostReactionDto>(itemCount, items);
    }

    public async Task<PostWithDetailsDto> GetWithDetailsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var result =  await PostRepository.GetAsync(id, includeDetails: true, cancellationToken);
        var item = ObjectMapper.Map<Post, PostWithDetailsDto>(result);
        return item;
    }

    public async Task CreatePostAsync(
        PostCUDto input,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var blobList = new List<Blob>();
        if (!input.Media.IsNullOrEmpty())
        {
            foreach (var item in input.Media!)
            {
                var fileBytes = await item.OpenReadStream().GetAllBytesAsync(cancellationToken);
                var blob = await BlobManager.SaveAsync(
                    userId,
                    item.FileName,
                    fileBytes,
                    cancellationToken);
                blobList.Add(blob);
            }
        }
        await PostManager.CreatePostAsync(
            userId,
            input.Visibility,
            input.Text,
            blobList,
            cancellationToken);
    }

    public async Task SetVisibilityAsync(
        Guid postId,
        PostVisibilityEnum visibilityEnum,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        await PostManager.SetVisibilityAsync(
            postId,
            userId,
            visibilityEnum,
            cancellationToken);
    }

    public async Task AddPostReactionAsync(
        Guid postId,
        ReactionTypeEnum reactionType,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        await PostManager.AddPostReactionAsync(
            userId,
            postId,
            reactionType,
            cancellationToken);
    }

    public async Task RemovePostReactionAsync(
        Guid postId,
        Guid reactionId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        await PostManager.RemovePostReactionAsync(
            postId,
            reactionId,
            userId,
            cancellationToken);
    }

    public async Task AddCommentAsync(
        Guid postId,
        CommentCUDto input,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        Blob? blob = null;
        if (input.Media is not null)
        {
            var fileBytes = await input.Media.OpenReadStream().GetAllBytesAsync(cancellationToken);
            blob = await BlobManager.SaveAsync(userId, input.Media.FileName, fileBytes, cancellationToken);
        }
        await PostManager.AddCommentAsync(
            userId,
            postId,
            input.ParentId,
            input.Text,
            blob,
            cancellationToken);
    }

    public async Task AddCommentReactionAsync(
        Guid postId,
        Guid commentId,
        ReactionTypeEnum reactionType,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        await PostManager.AddCommentReactionAsync(
            userId,
            postId,
            commentId,
            reactionType,
            cancellationToken);
    }

    public async Task RemoveCommentReactionAsync(
        Guid postId,
        Guid commentId,
        Guid reactionId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        await PostManager.RemoveCommentReactionAsync(
            postId,
            commentId,
            reactionId,
            userId,
            cancellationToken);
    }
}