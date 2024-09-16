using AutoMapper;
using Microsoft.AspNetCore.Http;
using ProLinked.Application.Contracts.Chats;
using ProLinked.Application.DTOs.Chats;
using ProLinked.Application.DTOs.Filtering;
using ProLinked.Domain;
using ProLinked.Domain.Contracts.Blobs;
using ProLinked.Domain.Contracts.Chats;
using ProLinked.Domain.DTOs.Chats;
using ProLinked.Domain.Entities.Blobs;
using ProLinked.Domain.Entities.Chats;
using ProLinked.Domain.Extensions;
using System.ComponentModel.DataAnnotations;

namespace ProLinked.Application.Services.Chats;

public class ChatService: ProLinkedServiceBase, IChatService
{
    private IChatManager ChatManager { get; }
    private IChatRepository ChatRepository { get; }
    private IBlobManager BlobManager { get; }
    private IRepository<Blob, Guid> BlobRepository { get; }

    public ChatService(
        IMapper mapper,
        IChatManager chatManager,
        IChatRepository chatRepository,
        IBlobManager blobManager,
        IRepository<Blob, Guid> blobRepository)
        : base(mapper)
    {
        ChatManager = chatManager;
        ChatRepository = chatRepository;
        BlobManager = blobManager;
        BlobRepository = blobRepository;
    }

    public async Task<IReadOnlyList<ChatLookUpDto>> GetListLookUpAsync(
        [Required] ListFilterDto input,
        [Required] Guid userId,
        CancellationToken cancellationToken = default)
    {
        var queryResult = await ChatRepository.GetLookUpListByUserOrderedByLastMessageAsync(
            userId,
            input.SkipCount,
            input.MaxResultCount,
            cancellationToken);

        var result = ObjectMapper.Map<List<ChatLookUp>, List<ChatLookUpDto>>(queryResult);
        return result.AsReadOnly();
    }

    public async Task<IReadOnlyList<MessageLookUpDto>> GetMessageListAsync(
        [Required] ChatListFilterDto input,
        [Required] Guid userId,
        CancellationToken cancellationToken = default)
    {
        _ = await ChatManager.GetChatAsync(userId, input.ChatId,cancellationToken);

        var messageList = await ChatRepository.GetMessageListAsync(
            input.ChatId,
            input.SkipCount,
            input.MaxResultCount,
            cancellationToken);

        var result = ObjectMapper.Map<List<MessageWithDetails>, List<MessageLookUpDto>>(messageList);
        return result.AsReadOnly();
    }

    public async Task<IReadOnlyList<ChatMembershipLookUpDto>> GetMemberListAsync(
        [Required] ChatListFilterDto input,
        [Required] Guid userId,
        CancellationToken cancellationToken = default)
    {
        _ = await ChatManager.GetChatAsync(userId, input.ChatId,cancellationToken);

        var membersList = await ChatRepository.GetMembersListAsync(
            input.ChatId,
            input.SkipCount,
            input.MaxResultCount,
            cancellationToken);

        var result = ObjectMapper.
            Map<List<ChatMembershipLookUp>, List<ChatMembershipLookUpDto>>(membersList);

        return result.AsReadOnly();
    }

    public async Task<ChatWithDetailsDto> GetDetailsAsync(
        [Required] Guid id,
        [Required] Guid userId,
        CancellationToken cancellationToken = default)
    {
        var chat = await ChatManager.GetChatAsync(userId, id, cancellationToken);

        var result = ObjectMapper.Map<Chat, ChatWithDetailsDto>(chat);

        return result;
    }

    public async Task AddMessageByUserAsync(
        [Required] MessageCreateByUserDto input,
        [Required] Guid userId,
        CancellationToken cancellationToken = default)
    {
        var newBlob = input.Media is not null
            ? await UploadBlobAsync(userId, input.Media, cancellationToken)
            : null;

        var currentDate = DateTime.Now;
        var chat = await ChatManager.CreateAsync(
            [userId, input.TargetUserId],
            cancellationToken: cancellationToken);

        var result = await ChatManager.AddMessageAsync(
                chat,
                userId,
                input.ParentId,
                input.Text,
                newBlob,
                cancellationToken);
        if (currentDate >= chat.CreationTime)
        {
            await ChatRepository.UpdateAsync(result, autoSave: true, cancellationToken);
            return;
        }
        await ChatRepository.InsertAsync(result, autoSave: true, cancellationToken);

    }

    public async Task AddMessageByChatAsync(
        [Required] MessageCreateByChatDto input,
        [Required] Guid userId,
        CancellationToken cancellationToken = default)
    {
        var newBlob = input.Media is not null
            ? await UploadBlobAsync(userId, input.Media, cancellationToken)
            : null ;
        var chat = await ChatManager.GetChatAsync(userId, input.ChatId, cancellationToken);

        var result = await ChatManager.AddMessageAsync(
                chat,
                userId,
                input.ParentId,
                input.Text,
                newBlob,
                cancellationToken);

        await ChatRepository.UpdateAsync(result, autoSave:true, cancellationToken);
    }

    public async Task AddMemberAsync(
        [Required] MemberCreateDto input,
        [Required] Guid userId,
        CancellationToken cancellationToken = default)
    {
        var chat = await ChatManager.GetChatAsync(userId, input.ChatId, cancellationToken);

        var result = await ChatManager.AddMemberAsync(chat, input.UserId, cancellationToken);
        await ChatRepository.UpdateAsync(result, autoSave: true, cancellationToken);
    }

    public async Task DeleteMemberAsync(
        [Required] Guid chatId,
        [Required] Guid userId,
        [Required] Guid memberId,
        CancellationToken cancellationToken = default)
    {
        var chat = await ChatManager.GetChatAsync(userId, chatId, cancellationToken);

        var result = await ChatManager.RemoveMemberAsync(chat, memberId, cancellationToken);
        if (result is not null)
        {
            await ChatRepository.UpdateAsync(result, autoSave: true, cancellationToken);
        }
    }

    public async Task CreateAsync(
        [Required] ChatCreateDto input,
        [Required] Guid userId,
        CancellationToken cancellationToken = default)
    {
        var newBlob = input.Image is not null
            ? await UploadBlobAsync(userId, input.Image, cancellationToken)
            : null;
        var currentDate = DateTime.Now;
        if (!input.UserIds.Contains(userId))
        {
            input.UserIds.Add(userId);
        }

        var result = await ChatManager.CreateAsync(
            input.UserIds,
            input.Title,
            newBlob,
            cancellationToken);
        if (result.CreationTime >= currentDate)
        {
            await ChatRepository.InsertAsync(result, autoSave: true, cancellationToken);
        }

    }

    public async Task UpdateTitleAsync(
        [Required] ChatUpdateTitleDto input,
        [Required] Guid userId,
        CancellationToken cancellationToken = default)
    {
        var chat = await ChatManager.GetChatAsync(userId, input.Id, cancellationToken);

        var result = await ChatManager.UpdateTitleAsync(chat, input.Title, cancellationToken);
        await ChatRepository.UpdateAsync(result, autoSave: true, cancellationToken);
    }

    public async Task UpdateImageAsync(
        [Required] ChatUpdateImageDto input,
        [Required] Guid userId,
        CancellationToken cancellationToken = default)
    {
        var newBlob = await UploadBlobAsync(userId, input.Image, cancellationToken);
        var chat = await ChatManager.GetChatAsync(userId, input.Id, cancellationToken);
        if (chat.Image is not null)
        {
            await BlobManager.DeleteAsync(chat.Image.StorageFileName, cancellationToken);
            await BlobRepository.DeleteAsync(chat.Image, autoSave:true, cancellationToken);
        }

        var result = await ChatManager.UpdateImageAsync(chat, newBlob, cancellationToken);
        await ChatRepository.UpdateAsync(result, autoSave: true, cancellationToken);
    }

    public async Task DeleteAsync(
        [Required] Guid id,
        [Required] Guid userId,
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

        await BlobRepository.InsertAsync(newBlob, autoSave:true, cancellationToken);
        return newBlob;
    }
}