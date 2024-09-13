using System.Collections.ObjectModel;
using ProLinked.Domain.Entities.Blobs;
using ProLinked.Domain.Extensions;
using ProLinked.Domain.Shared;
using ProLinked.Domain.Shared.Chats;
using ProLinked.Domain.Shared.Exceptions;
using ProLinked.Domain.Shared.Utils;

namespace ProLinked.Domain.Entities.Chats;

public class Chat: Entity<Guid>
{
    public string Title { get; private set; } = string.Empty;
    public ICollection<ChatMembership> Members { get; init; }
    public ICollection<Message> Messages { get; init; }
    public DateTime? LastMessageDate { get; private set; }
    public Blob? Image { get; private set; }
    public DateTime CreationTime { get; init; }

    private Chat(Guid id) : base(id){}

    public Chat(
        Guid id,
        string? title = null,
        DateTime? lastMessageDate = null,
        Blob? image = null,
        DateTime? creationTime = null)
        : base(id)
    {
        if (!title.IsNullOrWhiteSpace())
        {
            Title = title!;
        }

        if (image is not null)
        {
            Image = image;
        }

        Members = new Collection<ChatMembership>();
        Messages = new Collection<Message>();

        LastMessageDate = lastMessageDate;
        CreationTime = creationTime ?? DateTime.Now;
    }

    public Chat AddMember(ChatMembership member)
    {
        if (Members.Any(x => x.UserId == member.UserId))
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.MemberAlreadyExists)
                .WithData(nameof(ChatMembership.UserId), member.UserId);
        }

        Members.Add(member);

        return this;
    }

    public Chat RemoveMember(Guid memberId)
    {
        if (Members.All(x => x.UserId != memberId))
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.MemberNotFound)
                .WithData(nameof(ChatMembership.UserId), memberId);
        }

        var memberToDelete = Members.First(e => e.UserId == memberId);
        Members.Remove(memberToDelete);

        return this;
    }

    public Chat AddMessage(Message newMessage)
    {
        if (Messages.Any(x => x.Id ==  newMessage.Id))
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.MessageAlreadyExists)
                .WithData(nameof(Id), newMessage.Id);
        }
        Messages.Add(newMessage);
        LastMessageDate = newMessage.CreationTime;

        return this;
    }

    public Chat SetTitle(string title)
    {
        Check.Length(title, nameof(title), ChatConsts.MaxTitleLength, ChatConsts.MinTitleLength);

        Title = title;
        return this;
    }

    public Chat SetImage(Blob img)
    {
        Image = img;
        return this;
    }
}
