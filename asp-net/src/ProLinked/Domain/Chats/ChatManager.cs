using Microsoft.AspNetCore.Identity;
using ProLinked.Domain.AzureStorage.Blobs;
using ProLinked.Domain.Identity;
using ProLinked.Shared;
using ProLinked.Shared.AzureStorage;
using ProLinked.Shared.Exceptions;
using ProLinked.Shared.Extensions;

namespace ProLinked.Domain.Chats;

public class ChatManager: IDomainService
{
    private readonly IChatRepository _chatRepository;
    private readonly UserManager<AppUser> _userManager;

    public ChatManager(
        IChatRepository chatRepository,
        UserManager<AppUser> userManager)
    {
        _chatRepository = chatRepository;
        _userManager = userManager;
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

        return newChat;
    }

    public Chat AddMessage(
        Chat chat,
        Guid senderId,
        Guid? parentId = null,
        string? text = null,
        Blob? media = null)
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

        chat.AddMessage(newMessage);

        return chat;
    }

    public Chat UpdateImage(Chat chat, Blob newImg)
    {
        if (newImg.Type == BlobTypeEnum.Document)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.InvalidBlobType)
                .WithData(nameof(Message.Media.Id), newImg.Id)
                .WithData(nameof(Message.Media.Type), newImg.Type.ToString());
        }
        chat.SetImage(newImg);
        return chat;
    }

    public Chat UpdateTitle(Chat chat, string newTitle)
    {
        if (newTitle.IsNullOrWhiteSpace())
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.ChatTitleCannotBeNullOrWhitespace)
                .WithData(nameof(Chat.Id), chat.Id)
                .WithData(nameof(Chat.Title), newTitle);
        }

        chat.SetTitle(newTitle);
        return chat;
    }

    public async Task<Chat> AddMemberAsync(
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
        chat.AddMember(new ChatMembership(chat.Id, member.Id));
        return chat;
    }

    public async Task<Chat?> RemoveMemberAsync(
        Chat chat,
        Guid memberId,
        CancellationToken cancellationToken = default)
    {
        chat.RemoveMember(memberId);
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
