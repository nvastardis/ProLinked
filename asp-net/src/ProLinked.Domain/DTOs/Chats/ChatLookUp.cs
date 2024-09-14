namespace ProLinked.Domain.DTOs.Chats;

public class ChatLookUp
{
    public Guid Id { get; set; }
    public Guid? ImageId { get; set; }
    public string Title { get; set; } = null!;
    public string? LastMessageContent { get; set; }
    public string? LastMessageSenderName { get; set; }
    public DateTime? LastMessageDate { get; set; }
}
