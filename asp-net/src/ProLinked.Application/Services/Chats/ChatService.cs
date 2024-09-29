using AutoMapper;
using Microsoft.AspNetCore.Http;
using ProLinked.Application.Contracts.Chats;
using ProLinked.Application.Contracts.Chats.DTOs;
using ProLinked.Application.Contracts.Filtering;
using ProLinked.Application.DTOs;
using ProLinked.Domain.Contracts.Blobs;
using ProLinked.Domain.Contracts.Chats;
using ProLinked.Domain.Contracts.Notifications;
using ProLinked.Domain.DTOs.Chats;
using ProLinked.Domain.Entities.Blobs;
using ProLinked.Domain.Entities.Chats;
using ProLinked.Domain.Shared.Utils;

namespace ProLinked.Application.Services.Chats;

public class ChatService: ProLinkedServiceBase, IChatService
{
    private IChatManager ChatManager { get; }
    private IChatRepository ChatRepository { get; }
    private IBlobManager BlobManager { get; }
    private INotificationManager NotificationManager { get; }

    public ChatService(
        IMapper mapper,
        IChatManager chatManager,
        IChatRepository chatRepository,
        IBlobManager blobManager,
        INotificationManager notificationManager)
        : base(mapper)
    {
        ChatManager = chatManager;
        ChatRepository = chatRepository;
        BlobManager = blobManager;
        NotificationManager = notificationManager;
    }

    public async Task<PagedAndSortedResultList<ChatLookUpDto>> GetListLookUpAsync(
        ListFilterDto input,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var queryResult = await ChatRepository.GetLookUpListByUserOrderedByLastMessageAsync(
            userId,
            input.SkipCount,
            input.MaxResultCount,
            cancellationToken);

        var items = ObjectMapper.Map<List<ChatLookUp>, List<ChatLookUpDto>>(queryResult);
        var itemCount = items.Count;
        return new PagedAndSortedResultList<ChatLookUpDto>(itemCount, items.AsReadOnly());
    }

    public async Task<PagedAndSortedResultList<MessageLookUpDto>> GetMessageListAsync(
        ChatListFilterDto input,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        _ = await ChatManager.GetChatAsync(userId, input.ChatId,cancellationToken);

        var messageList = await ChatRepository.GetMessageListAsync(
            input.ChatId,
            input.SkipCount,
            input.MaxResultCount,
            cancellationToken);

        var items = ObjectMapper.Map<List<MessageWithDetails>, List<MessageLookUpDto>>(messageList);
        var itemCount = items.Count;
        return new PagedAndSortedResultList<MessageLookUpDto>(itemCount, items);
    }

    public async Task<PagedAndSortedResultList<ChatMembershipLookUpDto>> GetMemberListAsync(
        ChatListFilterDto input,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        _ = await ChatManager.GetChatAsync(userId, input.ChatId,cancellationToken);

        var membersList = await ChatRepository.GetMembersListAsync(
            input.ChatId,
            input.SkipCount,
            input.MaxResultCount,
            cancellationToken);

        var items = ObjectMapper.
            Map<List<ChatMembershipLookUp>, List<ChatMembershipLookUpDto>>(membersList);
        var itemCount = items.Count;
        return new PagedAndSortedResultList<ChatMembershipLookUpDto>(itemCount, items);
    }

    public async Task<ChatWithDetailsDto> GetDetailsAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var chat = await ChatManager.GetChatAsync(userId, id, cancellationToken);

        var result = ObjectMapper.Map<Chat, ChatWithDetailsDto>(chat);

        return result;
    }

    public async Task AddMessageByUserAsync(
        Guid targetUserId,
        MessageCreateDto input,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var newBlob = input.Media is not null
            ? await UploadBlobAsync(userId, input.Media, cancellationToken)
            : null;

        var chat = await ChatManager.CreateAsync(
            [userId, targetUserId],
            cancellationToken: cancellationToken);

        var result = await ChatManager.AddMessageAsync(
                chat,
                userId,
                input.ParentId,
                input.Text,
                newBlob,
                cancellationToken);

        await NotificationManager.CreateNotificationForMessageAsync(
            userId,
            targetUserId,
            result.Id,
            cancellationToken);
    }

    public async Task AddMessageByChatAsync(
        Guid chatId,
        MessageCreateDto input,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var newBlob = input.Media is not null
            ? await UploadBlobAsync(userId, input.Media, cancellationToken)
            : null ;
        var chat = await ChatManager.GetChatAsync(userId, chatId, cancellationToken);

        var result = await ChatManager.AddMessageAsync(
                chat,
                userId,
                input.ParentId,
                input.Text,
                newBlob,
                cancellationToken);

        foreach (var member in chat.Members)
        {
            await NotificationManager.CreateNotificationForMessageAsync(
                userId,
                member.UserId,
                result.Id,
                cancellationToken);
        }
    }

    public async Task AddMemberAsync(
        Guid chatId,
        MemberCreateDto input,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var chat = await ChatManager.GetChatAsync(userId, chatId, cancellationToken);

        await ChatManager.AddMemberAsync(chat, input.UserId, cancellationToken);
    }

    public async Task DeleteMemberAsync(
        Guid chatId,
        Guid userId,
        Guid memberId,
        CancellationToken cancellationToken = default)
    {
        var chat = await ChatManager.GetChatAsync(userId, chatId, cancellationToken);

        await ChatManager.RemoveMemberAsync(chat, memberId, cancellationToken);
    }

    public async Task CreateAsync(
        ChatCreateDto input,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var newBlob = input.Image is not null
            ? await UploadBlobAsync(userId, input.Image, cancellationToken)
            : null;

        if (!input.UserIds.Contains(userId))
        {
            input.UserIds.Add(userId);
        }

        await ChatManager.CreateAsync(
            input.UserIds,
            input.Title,
            newBlob,
            cancellationToken);
    }

    public async Task UpdateTitleAsync(
        Guid chatId,
        ChatUpdateTitleDto input,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var chat = await ChatManager.GetChatAsync(userId, chatId, cancellationToken);
        await ChatManager.UpdateTitleAsync(chat, input.Title, cancellationToken);
    }

    public async Task UpdateImageAsync(
        Guid chatId,
        ChatUpdateImageDto input,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var newBlob = await UploadBlobAsync(userId, input.Image, cancellationToken);
        var chat = await ChatManager.GetChatAsync(userId, chatId, cancellationToken);
        if (chat.Image is not null)
        {
            await BlobManager.DeleteAsync(chat.Image, cancellationToken);
        }

        await ChatManager.UpdateImageAsync(chat, newBlob, cancellationToken);
    }

    public async Task DeleteAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var chat = await ChatManager.GetChatAsync(userId, id, cancellationToken);
        await ChatRepository.DeleteAsync(chat, autoSave: true, cancellationToken);
    }

    private async Task<Blob> UploadBlobAsync(
        Guid userId,
        IFormFile file,
        CancellationToken cancellationToken = default)
    {
        var fileByteArray = await file.OpenReadStream().GetAllBytesAsync(cancellationToken);
        var fileName = file.FileName;
        var newBlob =  await BlobManager.SaveAsync(
            userId,
            fileName,
            fileByteArray,
            cancellationToken: cancellationToken);

        return newBlob;
    }
}