﻿using ProLinked.Domain.DTOs.Posts;
using ProLinked.Domain.Entities.Posts;

namespace ProLinked.Domain.Contracts.Posts;

public interface IPostRepository: IRepository<Post, Guid>
{
    Task<PostWithDetails> GetWithDetailsAsync(
        Guid postId,
        int skipCount = ProLinkedConsts.SkipCountDefaultValue,
        int maxResultCount = ProLinkedConsts.MaxResultCountDefaultValue,
        CancellationToken cancellationToken = default);

    Task<List<Post>> GetListAsync(
        Guid? userId,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        bool includeDetails = false,
        string? sorting = null,
        int skipCount = ProLinkedConsts.SkipCountDefaultValue,
        int maxResultCount = ProLinkedConsts.MaxResultCountDefaultValue,
        CancellationToken cancellationToken = default);

    Task<List<PostLookUp>> GetLookUpListAsync(
        Guid? postId = null,
        Guid? userId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        string? sorting = null,
        int skipCount = ProLinkedConsts.SkipCountDefaultValue,
        int maxResultCount = ProLinkedConsts.MaxResultCountDefaultValue,
        CancellationToken cancellationToken = default);

    Task<List<ReactionLookUp>> GetReactionsAsync(
        Guid postId,
        int skipCount = ProLinkedConsts.SkipCountDefaultValue,
        int maxResultCount = ProLinkedConsts.MaxResultCountDefaultValue,
        CancellationToken cancellationToken = default);

}