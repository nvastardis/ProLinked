namespace ProLinked.Domain.DTOs.Chats;

public class MessageWithDetails
{
    public Guid Id { get; set; }
    public Guid ChatId { get; set; }
    public Guid SenderId { get; set; }
    public DateTime CreationTime { get; set; }
    public string SenderFullName { get; set; }
    public string? Text { get; set; }
    public Guid? MediaId { get; set; } = null!;
}
