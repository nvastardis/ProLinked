using ProLinked.Application.DTOs.Chats;
using ProLinked.Application.DTOs.Filtering;
using System.ComponentModel.DataAnnotations;

namespace ProLinked.Application.Contracts.Chats;

public interface IChatService
{
    Task<IReadOnlyList<ChatLookUpDto>> GetListLookUpAsync(
        [Required] ListFilterDto input,
        [Required] Guid userId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MessageLookUpDto>> GetMessageListAsync(
        [Required] ChatListFilterDto input,
        [Required] Guid userId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ChatMembershipLookUpDto>> GetMemberListAsync(
        [Required] ChatListFilterDto input,
        [Required] Guid userId,
        CancellationToken cancellationToken = default);

    Task<ChatWithDetailsDto> GetDetailsAsync(
        [Required] Guid id,
        [Required] Guid userId,
        CancellationToken cancellationToken = default);

    Task AddMessageByChatAsync(
        [Required] MessageCreateByChatDto input,
        [Required] Guid userId,
        CancellationToken cancellationToken = default);

    Task AddMessageByUserAsync(
        [Required] MessageCreateByUserDto input,
        [Required] Guid userId,
        CancellationToken cancellationToken = default);

    Task AddMemberAsync(
        [Required] MemberCreateDto input,
        [Required] Guid userId,
        CancellationToken cancellationToken = default);

    Task DeleteMemberAsync(
        [Required] Guid chatId,
        [Required] Guid userId,
        [Required] Guid memberId,
        CancellationToken cancellationToken = default);

    Task CreateAsync(
        [Required] ChatCreateDto input,
        [Required] Guid userId,
        CancellationToken cancellationToken = default);

    Task UpdateTitleAsync(
        [Required] ChatUpdateTitleDto input,
        [Required] Guid userId,
        CancellationToken cancellationToken = default);

    Task UpdateImageAsync(
        [Required] ChatUpdateImageDto input,
        [Required] Guid userId,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        [Required] Guid id,
        [Required] Guid userId,
        CancellationToken cancellationToken = default);
}