using Microsoft.EntityFrameworkCore;
using ProLinked.Domain;
using ProLinked.Domain.Contracts.Posts;
using ProLinked.Domain.DTOs.Posts;
using ProLinked.Domain.Entities.Identity;
using ProLinked.Domain.Entities.Posts;
using ProLinked.Domain.Shared.Exceptions;
using ProLinked.Domain.Shared.Posts;
using ProLinked.Domain.Shared.Utils;

namespace ProLinked.Infrastructure.Data.Repositories.Posts;

public class PostRepository : ProLinkedBaseRepository<Post, Guid>, IPostRepository
{
    public PostRepository(
        ProLinkedDbContext dbContext)
        : base(dbContext)
    {
        DefaultSorting = $"{nameof(Post.CreationTime)} DESC";
    }

    public async Task<PostWithDetails> GetWithDetailsAsync(
        Guid postId,
        int skipCount = ProLinkedConsts.SkipCountDefaultValue,
        int maxResultCount = ProLinkedConsts.MaxResultCountDefaultValue,
        CancellationToken cancellationToken = default)
    {
        var postQueryable = await WithDetailsAsync(cancellationToken);
        var userQueryable = (await GetDbContextAsync(cancellationToken)).Set<AppUser>().AsQueryable();
        var commentQueryable = (await GetDbContextAsync(cancellationToken)).Set<Comment>().AsQueryable();

        var postQuery =
            from post in postQueryable
            join user in userQueryable on post.CreatorId equals user.Id
            where post.Id == postId
            select new PostWithDetails
            {
                Id = post.Id,
                CreatorId = post.CreatorId,
                CreatorFullName = $"{user.Name} {user.Surname}",
                CreatorProfilePhotoId = user.PhotographId,
                CreationTime = post.CreationTime,
                ReactionCount = post.Reactions.Count,
                Text = post.Text,
                MediaIds = post.Media == null ? null : post.Media!.Select(e => e.BlobId).ToList(),
                LastModificationTime = post.LastModificationDate
            };

        var postResult = await postQuery.SingleOrDefaultAsync(cancellationToken);
        if (postResult is null)
        {
            throw new EntityNotFoundException();
        }

        var commentQuery =
            from comment in commentQueryable
            join user in userQueryable on comment.CreatorId equals user.Id
            where comment.PostId == postId && comment.ParentId == null
            orderby comment.Reactions.Count descending
            select new CommentLookUp
            {
                Id = comment.Id,
                PostId = comment.PostId,
                CreatorId = comment.CreatorId,
                CreatorFullName = $"{user.Name} {user.Surname}",
                CreatorProfilePhotoId = user.PhotographId,
                CreationTime = comment.CreationTime,
                ReactionCount = comment.Reactions.Count,
                RepliesCount = comment.Children.Count,
                Text = comment.Text,
                MediaId = comment.Media == null ? null : comment.Media.Id,
                ParentId = comment.ParentId,
                LastModificationTime = comment.LastModificationTime
            };

        commentQuery =
            commentQuery
                .OrderByDescending(e => e.ReactionCount)
                .PageBy( skipCount, maxResultCount);

        var commentResult = await commentQuery.ToListAsync(cancellationToken);

        postResult.CommentCount = commentResult.Count;
        postResult.Comments = commentResult;

        var reactionResult = await GetReactionsAsync(
            postId,
            skipCount,
            maxResultCount,
            cancellationToken);

        postResult.Reactions = reactionResult;

        return postResult;
    }

    public async Task<List<Post>> GetListAsync(
        Guid? userId,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        PostVisibilityEnum visibilityEnum = PostVisibilityEnum.UNDEFINED,
        bool includeDetails = false,
        string? sorting = null,
        int skipCount = ProLinkedConsts.SkipCountDefaultValue,
        int maxResultCount = ProLinkedConsts.MaxResultCountDefaultValue,
        CancellationToken cancellationToken = default)
    {
        var filteredQuery = await FilterQueryableAsync(
            userId,
            fromDate,
            toDate,
            visibilityEnum,
            includeDetails,
            cancellationToken);

        filteredQuery = ApplyPagination(
            filteredQuery,
            sorting,
            skipCount,
            maxResultCount);

        return await filteredQuery.ToListAsync(cancellationToken);
    }

    public async Task<List<PostLookUp>> GetLookUpListAsync(
        Guid? userId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        PostVisibilityEnum visibilityEnum = PostVisibilityEnum.UNDEFINED,
        string? sorting = null,
        int skipCount = ProLinkedConsts.SkipCountDefaultValue,
        int maxResultCount = ProLinkedConsts.MaxResultCountDefaultValue,
        CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync(cancellationToken);
        var userQueryable = dbContext.Set<AppUser>().AsQueryable();

        var filteredQueryable = await FilterQueryableAsync(
            userId,
            fromDate,
            toDate,
            visibilityEnum,
            true,
            cancellationToken);

        filteredQueryable = ApplyPagination(
            filteredQueryable,
            sorting,
            skipCount,
            maxResultCount);

        var query =
            from post in filteredQueryable
            join user in userQueryable on post.CreatorId equals user.Id
            select new PostLookUp
            {
                Id = post.Id,
                CreatorId = post.CreatorId,
                CreatorFullName = $"{user.Name} {user.Surname}",
                CreatorProfilePhotoId = user.PhotographId,
                CreationTime = post.CreationTime,
                ReactionCount = post.Reactions.Count,
                CommentCount = post.Comments.Count,
                Text = post.Text,
                MediaIds = post.Media == null ? null : post.Media!.Select(e => e.BlobId).ToList(),
                LastModificationTime = post.LastModificationDate,
            };

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<List<ReactionLookUp>> GetReactionsAsync(
        Guid postId,
        int skipCount = ProLinkedConsts.SkipCountDefaultValue,
        int maxResultCount = ProLinkedConsts.MaxResultCountDefaultValue,
        CancellationToken cancellationToken = default)
    {
        var userQueryable = (await GetDbContextAsync(cancellationToken)).Set<AppUser>().AsQueryable();
        var reactionQueryable = (await GetDbContextAsync(cancellationToken)).Set<Reaction>().AsQueryable();

        var query = from reaction in reactionQueryable
            join user in userQueryable on reaction.CreatorId equals user.Id
            where reaction.PostId == postId
            select new ReactionLookUp
            {
                CreatorId = reaction.CreatorId,
                CreatorFullName = $"{user.Name} {user.Surname}",
                CreatorProfilePhotoId = user.PhotographId,
                CreationTime = reaction.CreationTime,
                ReactionType = reaction.ReactionType
            };
        query =
            query.
                OrderBy(e => e.CreationTime).
                PageBy(skipCount, maxResultCount);

        return await query.ToListAsync(cancellationToken);
    }

    public override async Task<IQueryable<Post>> WithDetailsAsync(CancellationToken cancellationToken = default)
    {
        var queryable = (await GetQueryableAsync(cancellationToken))
            .Include(e => e.Media)
            .Include(e => e.Reactions)
            .Include(e => e.Comments)
            .ThenInclude(e => e.Reactions);
        return queryable;
    }

    private async Task<IQueryable<Post>> FilterQueryableAsync(
        Guid? userId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        PostVisibilityEnum? visibilityEnum = PostVisibilityEnum.UNDEFINED,
        bool includeDetails = false,
        CancellationToken cancellationToken = default)
    {
        var queryable = includeDetails ? await WithDetailsAsync(cancellationToken) : await GetQueryableAsync(cancellationToken);

        var query = queryable
            .WhereIf(
                userId.HasValue,
                post => post.CreatorId == userId)
            .WhereIf(
                fromDate.HasValue,
                post => post.CreationTime >= fromDate)
            .WhereIf(
                toDate.HasValue,
                post => post.CreationTime <= toDate)
            .WhereIf(
                visibilityEnum.HasValue && visibilityEnum.Value != PostVisibilityEnum.UNDEFINED,
                post => post.PostVisibility == visibilityEnum);

        return query;
    }
}