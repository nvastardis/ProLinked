using Microsoft.AspNetCore.Identity;
using ProLinked.Domain.Contracts.Chats;
using ProLinked.Domain.Entities.Blobs;
using ProLinked.Domain.Entities.Chats;
using ProLinked.Domain.Entities.Identity;
using ProLinked.Domain.Shared;
using ProLinked.Domain.Shared.Blobs;
using ProLinked.Domain.Shared.Exceptions;
using ProLinked.Domain.Shared.Utils;

namespace ProLinked.Domain.Services;

public class ChatManager: IChatManager
{
    private readonly IChatRepository _chatRepository;
    private readonly IRepository<Message, Guid> _messageRepository;
    private readonly IRepository<ChatMembership> _chatMembershipRepository;
    private readonly UserManager<AppUser> _userManager;

    public ChatManager(
        IChatRepository chatRepository,
        UserManager<AppUser> userManager,
        IRepository<Message, Guid> messageRepository,
        IRepository<ChatMembership> chatMembershipRepository)
    {
        _chatRepository = chatRepository;
        _userManager = userManager;
        _messageRepository = messageRepository;
        _chatMembershipRepository = chatMembershipRepository;
    }

    public async Task<Chat> CreateAsync(
        List<Guid> memberIds,
        string? title = null,
        Blob? img = null,
        CancellationToken cancellationToken = default)
    {
        var chatFound =
            await _chatRepository.FindByUserListAsync(memberIds, true, cancellationToken);
        if (chatFound is not null)
        {
            return chatFound;
        }

        var userList = await FindUsersAsync(memberIds);
        cancellationToken.ThrowIfCancellationRequested();

        var newChat = new Chat(Guid.NewGuid());

        if (title.IsNullOrWhiteSpace())
        {
            var nameList =
                userList
                    .Select(e => e.Name + " " + e.Surname);
            var defaultTitle = string.Join(", ", nameList);
            newChat.SetTitle(defaultTitle);
        }
        else
        {
            newChat.SetTitle(title!);
        }

        if (img is not null)
        {
            newChat.SetImage(img);
        }

        var memberships =
            userList
                .Select(e => new ChatMembership(newChat.Id, e.Id));

        foreach (var membership in memberships)
        {
            newChat.AddMember(membership);
        }

        await _chatRepository.InsertAsync(newChat, autoSave: true, cancellationToken);
        return newChat;
    }

    public async Task<Message> AddMessageAsync(
        Chat chat,
        Guid senderId,
        Guid? parentId = null,
        string? text = null,
        Blob? media = null,
        CancellationToken cancellationToken = default)
    {
        if (text.IsNullOrWhiteSpace() && media is null)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.MessageCannotBeEmpty)
                .WithData(nameof(Message.ChatId), chat.Id)
                .WithData(nameof(Message.SenderId), senderId);
        }

        var parent = chat.Messages.FirstOrDefault(e => e.Id == (parentId ?? Guid.Empty));
        if (parentId.HasValue && parent is null)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.ParentMessageNotFound)
                .WithData(nameof(Message.Parent), parentId);
        }

        var newMessage = new Message(
            Guid.NewGuid(),
            chat.Id,
            senderId,
            parent,
            text,
            media);

        cancellationToken.ThrowIfCancellationRequested();
        chat.AddMessage(newMessage);
        await _messageRepository.InsertAsync(newMessage, autoSave: true, cancellationToken);
        await _chatRepository.UpdateAsync(chat, autoSave:true, cancellationToken);

        return newMessage;
    }

    public async Task UpdateImageAsync(
        Chat chat,
        Blob newImg,
        CancellationToken cancellationToken = default)
    {
        if (newImg.Type == BlobTypeEnum.Document)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.InvalidBlobType)
                .WithData(nameof(Message.Media.Id), newImg.Id)
                .WithData(nameof(Message.Media.Type), newImg.Type.ToString());
        }

        cancellationToken.ThrowIfCancellationRequested();
        chat.SetImage(newImg);
        await _chatRepository.UpdateAsync(chat, autoSave: true, cancellationToken);
    }

    public async Task UpdateTitleAsync(
        Chat chat,
        string newTitle,
        CancellationToken cancellationToken = default)
    {
        if (newTitle.IsNullOrWhiteSpace())
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.ChatTitleCannotBeNullOrWhitespace)
                .WithData(nameof(Chat.Id), chat.Id)
                .WithData(nameof(Chat.Title), newTitle);
        }

        cancellationToken.ThrowIfCancellationRequested();
        chat.SetTitle(newTitle);
        await _chatRepository.UpdateAsync(chat, autoSave: true, cancellationToken);
    }

    public async Task<ChatMembership> AddMemberAsync(
        Chat chat,
        Guid memberId,
        CancellationToken cancellationToken = default)
    {
        var member = await FindUserAsync(memberId);
        if (chat.Members.Any(e => e.UserId == memberId))
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.MemberAlreadyExists)
                .WithData(nameof(Chat.Id), chat.Id)
                .WithData(nameof(ChatMembership.UserId), memberId);
        }

        cancellationToken.ThrowIfCancellationRequested();
        var newMember = new ChatMembership(chat.Id, member.Id);
        chat.AddMember(newMember);
        await _chatMembershipRepository.InsertAsync(newMember, autoSave: true, cancellationToken);
        return newMember;
    }

    public async Task<Chat?> RemoveMemberAsync(
        Chat chat,
        Guid memberId,
        CancellationToken cancellationToken = default)
    {
        chat.RemoveMember(memberId);
        await _chatMembershipRepository.DeleteManyAsync(e => e.UserId == memberId, autoSave: true, cancellationToken);
        if (chat.Members.Count > 1)
        {
            return chat;
        }

        await _chatRepository.DeleteAsync(chat, autoSave: true, cancellationToken:cancellationToken);
        return null;

    }

    public async Task<Chat> GetChatAsync(
        Guid userId,
        Guid chatId,
        CancellationToken cancellationToken = default)
    {
        var chat = await _chatRepository.GetAsync(chatId, true, cancellationToken);
        if(chat.Members.All(e => e.UserId != userId))
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.MemberNotFound);
        }
        return chat;
    }

    public async Task DeleteAsync(
        Guid chatId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var chat = await GetChatAsync(userId, chatId, cancellationToken);
        await _chatRepository.DeleteAsync(chat, autoSave: true, cancellationToken);
    }


    private async Task<AppUser> FindUserAsync(
        Guid userId)
    {
        var userToAdd = await _userManager.FindByIdAsync(userId.ToString());
        if (userToAdd is null)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.UserNotFound)
                .WithData(nameof(AppUser.Id), userId);
        }

        return userToAdd;
    }

    private async Task<IReadOnlyList<AppUser>> FindUsersAsync(
        IEnumerable<Guid> userIds)
    {
        var userList = new List<AppUser>();
        foreach (var userId in userIds)
        {
            var userToAdd = await FindUserAsync(userId);
            userList.Add(userToAdd);
        }

        return userList.AsReadOnly();
    }
}