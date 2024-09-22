using AutoMapper;
using ProLinked.Application.Contracts.Filtering;
using ProLinked.Application.Contracts.Posts;
using ProLinked.Application.Contracts.Posts.DTOs;
using ProLinked.Application.DTOs;
using ProLinked.Domain.Contracts.Blobs;
using ProLinked.Domain.Contracts.Posts;
using ProLinked.Domain.DTOs.Posts;
using ProLinked.Domain.Entities.Blobs;
using ProLinked.Domain.Entities.Posts;
using ProLinked.Domain.Shared.Posts;
using ProLinked.Domain.Shared.Utils;

namespace ProLinked.Application.Services.Posts;

public class CommentService: ProLinkedServiceBase, ICommentService
{
    private readonly IBlobManager BlobManager;
    private readonly IPostManager PostManager;
    private readonly IPostRepository PostRepository;
    private readonly ICommentRepository CommentRepository;

    public CommentService(
        IMapper objectMapper,
        IPostManager postManager,
        IBlobManager blobManager,
        IPostRepository postRepository,
        ICommentRepository commentRepository)
        : base(objectMapper)
    {
        PostManager = postManager;
        BlobManager = blobManager;
        PostRepository = postRepository;
        CommentRepository = commentRepository;
    }

    public async Task<PagedAndSortedResultList<CommentDto>> GetCommentListForPostAsync(
        CommentListFilterDto filterDto,
        CancellationToken cancellationToken = default)
    {
        var queryResult = await CommentRepository.GetListAsync(
            filterDto.PostId,
            filterDto.UserId,
            filterDto.From,
            filterDto.To,
             true,
            filterDto.Sorting,
            filterDto.SkipCount,
            filterDto.MaxResultCount,
            cancellationToken);

        var items = ObjectMapper.Map<List<Comment>, List<CommentDto>>(queryResult);
        var itemCount = items.Count;
        return new PagedAndSortedResultList<CommentDto>(itemCount, items);
    }

    public async Task CreateCommentAsync(
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

        var post = await PostRepository.GetAsync(
            input.PostId,
            includeDetails: true,
            cancellationToken);

        await PostManager.AddCommentAsync(
            post,
            userId,
            input.ParentId,
            input.Text,
            blob,
            cancellationToken);
    }

    public async Task UpdateCommentAsync(
        CommentCUDto input,
        Guid commentId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var comment = await PostManager.GetCommentAsCreatorAsync(
            commentId,
            userId,
            cancellationToken);

        Blob? blob = null;
        if (input.Media is not null)
        {
            var fileBytes = await input.Media.OpenReadStream().GetAllBytesAsync(cancellationToken);
            blob = await BlobManager.SaveAsync(userId, input.Media.FileName, fileBytes, cancellationToken);
        }

        await PostManager.UpdateCommentAsync(
            comment,
            input.Text,
            blob,
            cancellationToken);
    }

    public async Task DeleteCommentAsync(
        Guid commentId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var comment = await PostManager.GetCommentAsCreatorAsync(
            commentId,
            userId,
            cancellationToken);

        await CommentRepository.DeleteAsync(comment, autoSave: true, cancellationToken);
    }


    public async Task<PagedAndSortedResultList<ReactionDto>> GetCommentReactionListAsync(
        Guid commentId,
        ListFilterDto filterDto,
        CancellationToken cancellationToken = default)
    {
        var queryResult = await CommentRepository.GetReactionsAsync(
            commentId,
            filterDto.SkipCount,
            filterDto.MaxResultCount,
            cancellationToken);

        var items = ObjectMapper.Map<List<ReactionLookUp>, List<ReactionDto>>(queryResult);
        var itemCount = items.Count;
        return new PagedAndSortedResultList<ReactionDto>(itemCount, items);
    }

    public async Task CreateCommentReactionAsync(
        Guid commentId,
        Guid userId,
        ReactionTypeEnum reactionType,
        CancellationToken cancellationToken = default)
    {
        var comment = await CommentRepository.GetAsync(
            commentId,
            includeDetails: true,
            cancellationToken);

        await PostManager.AddCommentReactionAsync(
            comment,
            userId,
            reactionType,
            cancellationToken);
    }

    public async Task DeleteCommentReactionAsync(
        Guid commentId,
        Guid reactionId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var comment = await CommentRepository.GetAsync(
            commentId,
            includeDetails: true,
            cancellationToken);

        var commentReaction = await PostManager.GetCommentReactionAsCreatorAsync(
            reactionId,
            userId,
            cancellationToken);

        await PostManager.RemoveCommentReactionAsync(
            comment,
            commentReaction,
            cancellationToken);
    }
}