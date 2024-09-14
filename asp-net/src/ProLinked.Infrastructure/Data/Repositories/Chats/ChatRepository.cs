using Microsoft.EntityFrameworkCore;
using ProLinked.Domain;
using ProLinked.Domain.Contracts.Chats;
using ProLinked.Domain.DTOs.Chats;
using ProLinked.Domain.Entities.Chats;
using ProLinked.Domain.Entities.Identity;
using ProLinked.Domain.Extensions;
using ProLinked.Infrastructure.Data;

namespace ProLinked.Infrastructure.Data.Repositories.Chats;

public class ChatRepository: ProLinkedBaseRepository<Chat, Guid>, IChatRepository
{
    public ChatRepository(
        ProLinkedDbContext dbContext)
        : base(dbContext)
    {
        DefaultSorting = $"{nameof(Chat.LastMessageDate)} DESC";
    }

    public async Task<List<Chat>> GetListByUserAsync(
        Guid memberId,
        bool includeDetails = false,
        string? sorting = null,
        int skipCount = ProLinkedConsts.SkipCountDefaultValue,
        int maxResultCount = ProLinkedConsts.MaxResultCountDefaultValue,
        CancellationToken cancellationToken = default)
    {
        var filteredQuery = await FilterQueryableAsync(memberId:memberId, includeDetails:includeDetails, cancellationToken: cancellationToken);
        filteredQuery = ApplyPagination(
            filteredQuery,
            sorting,
            skipCount,
            maxResultCount);
        return await filteredQuery.ToListAsync(cancellationToken);
    }

    public async Task<List<MessageWithDetails>> GetMessageListAsync(
        Guid chatId,
        int skipCount = ProLinkedConsts.SkipCountDefaultValue,
        int maxResultCount = ProLinkedConsts.MaxResultCountDefaultValue,
        CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync(cancellationToken);
        var chatQueryable = await GetQueryableAsync(cancellationToken);
        var messageQueryable = dbContext.Set<Message>().AsQueryable();
        var userQueryable = dbContext.Set<AppUser>().AsQueryable();

        var query =
            from chat in chatQueryable
            join message in messageQueryable on chat.Id equals message.ChatId into messageInfo
            from message in messageInfo.DefaultIfEmpty()
            join user in userQueryable on message.SenderId equals user.Id
            where chat.Id == chatId
            select new MessageWithDetails()
            {
                Id = message.Id,
                ChatId = chatId,
                SenderId = user.Id,
                CreationTime = message.CreationTime,
                MediaId = message.Media.Id,
                SenderFullName = user.Name + " " + user.Surname,
                Text = message.Text
            };
        var result = ApplyMessagePagination(query, skipCount, maxResultCount);
        return await result.ToListAsync(cancellationToken);
    }

    public async Task<List<ChatMembershipLookUp>> GetMembersListAsync(
        Guid chatId,
        int skipCount = ProLinkedConsts.SkipCountDefaultValue,
        int maxResultCount = ProLinkedConsts.MaxResultCountDefaultValue,
        CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync(cancellationToken);
        var chatQueryable = await GetQueryableAsync(cancellationToken);
        var memberQueryable = dbContext.Set<ChatMembership>().AsQueryable();
        var userQueryable = dbContext.Set<AppUser>().AsQueryable();

        var query =
            from chat in chatQueryable
            join member in memberQueryable on chat.Id equals member.ChatId
            join user in userQueryable on member.UserId equals user.Id
            where chat.Id == chatId
            select new ChatMembershipLookUp()
            {
                ChatId = chatId,
                UserId = user.Id,
                CreationTime = member.CreationTime,
                UserFullName = user.Name + " " + user.Surname,
                PhotoId = user.PhotographId ?? Guid.Empty
            };
        var result = ApplyMembersPagination(query, skipCount, maxResultCount);
        return await result.ToListAsync(cancellationToken);
    }

    public async Task<List<ChatLookUp>> GetLookUpListByUserOrderedByLastMessageAsync(
        Guid memberId,
        int skipCount = ProLinkedConsts.SkipCountDefaultValue,
        int maxResultCount = ProLinkedConsts.MaxResultCountDefaultValue,
        CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync(cancellationToken);
        var chatQueryable = await GetQueryableAsync(cancellationToken);
        var messageQueryable = dbContext.Set<Message>().AsQueryable();
        var userQueryable = dbContext.Set<AppUser>().AsQueryable();
        var membershipQueryable = dbContext.Set<ChatMembership>().AsQueryable();

        var query =
            from chat in chatQueryable
            join member in membershipQueryable on chat.Id equals member.ChatId
            join message in messageQueryable on chat.Id equals message.ChatId into messageInfo
            from messageItem in messageInfo.DefaultIfEmpty()
            join user in userQueryable on messageItem.SenderId equals user.Id into userInfo
            from userItem in userInfo.DefaultIfEmpty()
            where memberId == member.UserId && (!chat.LastMessageDate.HasValue || messageItem.CreationTime == chat.LastMessageDate)
            select new ChatLookUp()
            {
                Id = chat.Id,
                Title = chat.Title,
                ImageId = chat.Image.Id,
                LastMessageDate = chat.LastMessageDate,
                LastMessageContent = messageItem.Text,
                LastMessageSenderName = userItem.Name
            };
        query = query.
            OrderByDescending(e=>e.LastMessageDate).
            PageBy(skipCount, maxResultCount);
        return await query.ToListAsync(cancellationToken);

    }

    public async Task<Chat?> FindByUserListAsync(
        List<Guid> members,
        bool includeDetails = false,
        CancellationToken cancellationToken = default)
    {
        var filteredQueryable = await FilterQueryableAsync(members:members, includeDetails:includeDetails, cancellationToken: cancellationToken);
        var result = filteredQueryable.Any() ? await filteredQueryable.FirstAsync(cancellationToken) : null;
        return result;
    }

    public override async Task<IQueryable<Chat>> WithDetailsAsync(CancellationToken cancellationToken = default)
    {
        return (await GetQueryableAsync(cancellationToken))
            .Include(e => e.Members)
            .Include(e => e.Messages)
            .Include(e => e.Image);
    }

    private async Task<IQueryable<Chat>> FilterQueryableAsync(
        Guid? memberId = null,
        List<Guid>? members = null,
        bool includeDetails = false,
        CancellationToken cancellationToken = default)
    {
        var queryable = includeDetails ? await WithDetailsAsync(cancellationToken) : await GetQueryableAsync(cancellationToken);

        var query = queryable
            .WhereIf(
                memberId.HasValue && memberId.Value != Guid.Empty,
                chat => chat.Members.Any(e => e.UserId == memberId))
            .WhereIf(
                members is not null && members.Any(),
                chat =>
                    chat.Members.All(e => members!.Contains(e.UserId)) &&
                    members!.All(e => chat.Members.Select(u => u.UserId).Contains(e)));

        return query;
    }

    private IAsyncEnumerable<MessageWithDetails> ApplyMessagePagination(
        IQueryable<MessageWithDetails> collection,
        int skipCount = ProLinkedConsts.SkipCountDefaultValue,
        int maxResultCount = ProLinkedConsts.MaxResultCountDefaultValue)
    {
        var query = collection.AsQueryable();
        query = query.OrderByDescending(e => e.CreationTime);

        return query.PageBy(skipCount, maxResultCount).ToAsyncEnumerable();
    }

    private IAsyncEnumerable<ChatMembershipLookUp> ApplyMembersPagination(
        IQueryable<ChatMembershipLookUp> collection,
        int skipCount = ProLinkedConsts.SkipCountDefaultValue,
        int maxResultCount = ProLinkedConsts.MaxResultCountDefaultValue)
    {
        var query = collection.AsQueryable();
        query = query.OrderByDescending(e => e.CreationTime);
        return query.PageBy(skipCount, maxResultCount).ToAsyncEnumerable();
    }
}