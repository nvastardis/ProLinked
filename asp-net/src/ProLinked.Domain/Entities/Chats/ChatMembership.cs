namespace ProLinked.Domain.Entities.Chats;

public class ChatMembership : Entity
{
    public Guid ChatId { get; init; }
    public Guid UserId { get; init; }
    public DateTime CreationTime { get; init; }

    private ChatMembership() {}

    public ChatMembership(
        Guid chatId,
        Guid userId,
        DateTime? creationTime = null)
    {
        ChatId = chatId;
        UserId = userId;
        CreationTime = creationTime ?? DateTime.Now;
    }

    public override object?[] GetKeys()
    {
        return [ChatId, UserId];
    }
}
