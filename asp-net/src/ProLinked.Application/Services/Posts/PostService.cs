using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using ProLinked.Application.Contracts.Posts;
using ProLinked.Application.DTOs;
using ProLinked.Application.DTOs.Filtering;
using ProLinked.Application.DTOs.Posts;
using ProLinked.Domain.Contracts.Blobs;
using ProLinked.Domain.Contracts.Posts;
using ProLinked.Domain.DTOs.Posts;
using ProLinked.Domain.Entities.Blobs;
using ProLinked.Domain.Entities.Posts;
using ProLinked.Domain.Extensions;
using ProLinked.Domain.Shared.Posts;

namespace ProLinked.Application.Services.Posts;

public class PostService: ProLinkedServiceBase, IPostService
{
    private readonly IPostManager PostManager;
    private readonly IBlobManager BlobManager;
    private readonly IPostRepository PostRepository;

    public PostService(
        IMapper objectMapper,
        IPostManager postManager,
        IPostRepository postRepository,
        IBlobManager blobManager)
        : base(objectMapper)
    {
        PostManager = postManager;
        PostRepository = postRepository;
        BlobManager = blobManager;
    }

    public async Task<PagedAndSortedResultList<PostLookUpDto>> GetPostListAsync(
        PostListFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        var queryResult = await PostRepository.GetLookUpListAsync(
            null,
            filter.UserId,
            filter.From,
            filter.To,
            filter.Sorting,
            filter.SkipCount,
            filter.MaxResultCount,
            cancellationToken);

        var items = ObjectMapper.Map<List<PostLookUp>, List<PostLookUpDto>>(queryResult);
        var itemCount = items.Count;
        return new PagedAndSortedResultList<PostLookUpDto>(itemCount, items);
    }

    public async Task<PostWithDetailsDto> GetWithDetailsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var result =  await PostRepository.GetAsync(id, includeDetails:true, cancellationToken);
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

    public async Task UpdatePostAsync(
        PostCUDto input,
        Guid postId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var post = await PostManager.GetPostAsCreatorAsync(
            postId,
            userId,
            cancellationToken);

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

        await PostManager.UpdatePostAsync(
            post,
            input.Text,
            blobList,
            cancellationToken);
    }

    public async Task SetVisibilityAsync(
        Guid postId,
        Guid userId,
        PostVisibilityEnum visibilityEnum,
        CancellationToken cancellationToken = default)
    {
        var post = await PostManager.GetPostAsCreatorAsync(
            postId,
            userId,
            cancellationToken);
        await PostManager.SetVisibilityAsync(
            post,
            visibilityEnum,
            cancellationToken);
    }

    public async Task DeletePostAsync(
        Guid postId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var post = await PostManager.GetPostAsCreatorAsync(
            postId,
            userId,
            cancellationToken);

        await PostRepository.DeleteAsync(
            post,
            autoSave: true,
            cancellationToken);
    }


    public async Task<PagedAndSortedResultList<ReactionDto>> GetPostReactionListAsync(
        Guid postId,
        ListFilterDto filterDto,
        CancellationToken cancellationToken = default)
    {
        var post = await PostRepository.GetAsync(postId, includeDetails: true, cancellationToken);
        var items = ObjectMapper.Map<List<Reaction>, List<ReactionDto>>(post.Reactions.ToList());
        var itemCount = items.Count;
        return new PagedAndSortedResultList<ReactionDto>(itemCount, items);
    }

    public async Task CreatePostReactionAsync(
        Guid postId,
        Guid userId,
        ReactionTypeEnum reactionType,
        CancellationToken cancellationToken = default)
    {
        var post = await PostRepository.GetAsync(
            postId,
            includeDetails: true,
            cancellationToken);

        await PostManager.AddPostReactionAsync(
            post,
            userId,
            reactionType,
            cancellationToken);
    }

    public async Task DeletePostReactionAsync(
        Guid postId,
        Guid reactionId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var post = await PostRepository.GetAsync(
            postId,
            includeDetails: true,
            cancellationToken);

        var reaction = await PostManager.GetPostReactionAsCreatorAsync(
            reactionId,
            userId,
            cancellationToken);

        await PostManager.RemovePostReactionAsync(
            post,
            reaction,
            cancellationToken);
    }
}