using ProLinked.Application.DTOs;
using ProLinked.Application.DTOs.Chats;
using ProLinked.Application.DTOs.Filtering;

namespace ProLinked.Application.Contracts.Chats;

public interface IChatService
{
    Task<PagedAndSortedResultList<ChatLookUpDto>> GetListLookUpAsync(
        ListFilterDto input,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<PagedAndSortedResultList<MessageLookUpDto>> GetMessageListAsync(
        ChatListFilterDto input,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<PagedAndSortedResultList<ChatMembershipLookUpDto>> GetMemberListAsync(
        ChatListFilterDto input,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<ChatWithDetailsDto> GetDetailsAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task AddMessageByChatAsync(
        Guid chatId,
        MessageCreateDto input,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task AddMessageByUserAsync(
        Guid targetUserId,
        MessageCreateDto input,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task AddMemberAsync(
        Guid chatId,
        MemberCreateDto input,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task DeleteMemberAsync(
        Guid chatId,
        Guid userId,
        Guid memberId,
        CancellationToken cancellationToken = default);

    Task CreateAsync(
        ChatCreateDto input,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task UpdateTitleAsync(
        Guid chatId,
        ChatUpdateTitleDto input,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task UpdateImageAsync(
        Guid chatId,
        ChatUpdateImageDto input,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default);
}