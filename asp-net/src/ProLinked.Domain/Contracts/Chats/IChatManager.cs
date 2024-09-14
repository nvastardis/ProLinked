using ProLinked.Domain.Entities.Blobs;
using ProLinked.Domain.Entities.Chats;

namespace ProLinked.Domain.Contracts.Chats;

public interface IChatManager
{
    Task<Chat> CreateAsync(
        List<Guid> memberIds,
        string? title = null,
        Blob? img = null,
        CancellationToken cancellationToken = default);

    Task<Chat> AddMessageAsync(
        Chat chat,
        Guid senderId,
        Guid? parentId = null,
        string? text = null,
        Blob? media = null,
        CancellationToken cancellationToken = default);

    Task<Chat> UpdateImageAsync(
        Chat chat,
        Blob newImg,
        CancellationToken cancellationToken = default);

    Task<Chat> UpdateTitleAsync(
        Chat chat,
        string newTitle,
        CancellationToken cancellationToken = default);

    Task<Chat> AddMemberAsync(
        Chat chat,
        Guid memberId,
        CancellationToken cancellationToken = default);

    Task<Chat?> RemoveMemberAsync(
        Chat chat,
        Guid memberId,
        CancellationToken cancellationToken = default);

    Task<Chat> GetChatAsync(
        Guid userId,
        Guid chatId,
        CancellationToken cancellationToken = default);
}