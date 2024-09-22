using Microsoft.EntityFrameworkCore;
using ProLinked.Domain;
using ProLinked.Domain.Contracts.Posts;
using ProLinked.Domain.DTOs.Posts;
using ProLinked.Domain.Entities.Identity;
using ProLinked.Domain.Entities.Posts;
using ProLinked.Domain.Extensions;
using ProLinked.Domain.Shared.Utils;

namespace ProLinked.Infrastructure.Data.Repositories.Posts;

public class CommentRepository: ProLinkedBaseRepository<Comment, Guid>, ICommentRepository
{
    public CommentRepository(
        ProLinkedDbContext dbContext)
        : base(dbContext)
    {
        DefaultSorting = $"{nameof(Comment.CreationTime)} DESC";
    }

    public async Task<List<Comment>> GetListAsync(
        Guid? postId = null,
        Guid? userId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        bool includeDetails = false,
        string? sorting = null,
        int skipCount = ProLinkedConsts.SkipCountDefaultValue,
        int maxResultCount = ProLinkedConsts.MaxResultCountDefaultValue,
        CancellationToken cancellationToken = default)
    {
        var filteredQuery = await FilterQueryableAsync(
            postId,
            userId,
            fromDate,
            toDate,
            includeDetails,
            cancellationToken);

        filteredQuery = ApplyPagination(
            filteredQuery,
            sorting,
            skipCount,
            maxResultCount);

        return await filteredQuery.ToListAsync(cancellationToken);
    }

    public async Task<List<CommentLookUp>> GetLookUpListAsync(
        Guid? postId = null,
        Guid? userId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        string? sorting = null,
        int skipCount = ProLinkedConsts.SkipCountDefaultValue,
        int maxResultCount = ProLinkedConsts.MaxResultCountDefaultValue,
        CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync(cancellationToken);
        var userQueryable = dbContext.Set<AppUser>().AsQueryable();

        var filteredQueryable = await FilterQueryableAsync(
            postId,
            userId,
            fromDate,
            toDate,
            true,
            cancellationToken);

        filteredQueryable = ApplyPagination(
            filteredQueryable,
            sorting,
            skipCount,
            maxResultCount);

        var query  =
            from comment in filteredQueryable
            join user in userQueryable on comment.CreatorId equals user.Id
            select new CommentLookUp()
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

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<List<ReactionLookUp>> GetReactionsAsync(
        Guid commentId,
        int skipCount = ProLinkedConsts.SkipCountDefaultValue,
        int maxResultCount = ProLinkedConsts.MaxResultCountDefaultValue,
        CancellationToken cancellationToken = default)
    {
        var userQueryable = (await GetDbContextAsync(cancellationToken)).Set<AppUser>().AsQueryable();
        var reactionQueryable = (await GetDbContextAsync(cancellationToken)).Set<Reaction>().AsQueryable();

        var query = from reaction in reactionQueryable
                    join user in userQueryable on reaction.CreatorId equals user.Id
                    where reaction.CommentId == commentId
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

    private async Task<IQueryable<Comment>> FilterQueryableAsync(
        Guid? postId = null,
        Guid? userId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        bool includeDetails = false,
        CancellationToken cancellationToken = default)
    {
        var queryable = includeDetails ?
            await WithDetailsAsync(cancellationToken) :
            await GetQueryableAsync(cancellationToken);

        var query = queryable
            .WhereIf(
                postId.HasValue,
                comment => comment.PostId == postId)
            .WhereIf(
                userId.HasValue,
                comment => comment.CreatorId == userId)
            .WhereIf(
                fromDate.HasValue,
                comment => comment.CreationTime >= fromDate)
            .WhereIf(
                toDate.HasValue,
                comment => comment.CreationTime <= toDate);

        return query;
    }

    public override async Task<IQueryable<Comment>> WithDetailsAsync(CancellationToken cancellationToken = default)
    {
        var queryable = (await GetQueryableAsync(cancellationToken))
            .Include(e => e.Media)
            .Include(e => e.Reactions)
            .Include(e => e.Children);
        return queryable;
    }
}