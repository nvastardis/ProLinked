using ProLinked.Domain.DTOs.Chats;
using ProLinked.Domain.Entities.Chats;

namespace ProLinked.Domain.Contracts.Chats;

public interface IChatRepository: IRepository<Chat, Guid>
{
   Task<List<Chat>> GetListByUserAsync(
        Guid memberId,
        bool includeDetails = false,
        string? sorting = null,
        int skipCount = ProLinkedConsts.SkipCountDefaultValue,
        int maxResultCount = ProLinkedConsts.MaxResultCountDefaultValue,
        CancellationToken cancellationToken = default);

   Task<List<MessageWithDetails>> GetMessageListAsync(
       Guid chatId,
       int skipCount = ProLinkedConsts.SkipCountDefaultValue,
       int maxResultCount = ProLinkedConsts.MaxResultCountDefaultValue,
       CancellationToken cancellationToken = default);

   Task<List<ChatMembershipLookUp>> GetMembersListAsync(
       Guid chatId,
       int skipCount = ProLinkedConsts.SkipCountDefaultValue,
       int maxResultCount = ProLinkedConsts.MaxResultCountDefaultValue,
       CancellationToken cancellationToken = default);

   Task<List<ChatLookUp>> GetLookUpListByUserOrderedByLastMessageAsync(
        Guid memberId,
        int skipCount = ProLinkedConsts.SkipCountDefaultValue,
        int maxResultCount = ProLinkedConsts.MaxResultCountDefaultValue,
        CancellationToken cancellationToken = default);

   Task<Chat?> FindByUserListAsync(
        List<Guid> memberId,
        bool includeDetails = false,
        CancellationToken cancellationToken = default);
}