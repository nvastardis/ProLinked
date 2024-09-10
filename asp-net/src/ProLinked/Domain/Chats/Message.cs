using ProLinked.Domain.AzureStorage.Blobs;
using ProLinked.Shared.Chats;
using ProLinked.Shared.Extensions;
using ProLinked.Shared.Utils;

namespace ProLinked.Domain.Chats;

public class Message: Entity<Guid>
{
    public Guid ChatId { get; init; }
    public Guid SenderId { get; init; }
    public DateTime CreationTime { get; init; }
    public Message? Parent { get; init; }
    public string? Text { get; private set; }
    public Blob? Media { get; private set; }

    private Message(Guid id): base(id) {}

    public Message(
        Guid id,
        Guid chatId,
        Guid senderId,
        Message? parent = null,
        string? text = null,
        Blob? media = null,
        DateTime? creationTime = null)
        : base(id)
    {
        ChatId = chatId;
        SenderId = senderId;
        Parent = parent;
        SetContent(text, media);
        CreationTime = creationTime ?? DateTime.Now;
    }

    private void SetContent(string? text = null, Blob? media = null)
    {
        if (!text.IsNullOrWhiteSpace())
        {
            SetText(text!);
        }

        if (media is not null)
        {
            SetMedia(media);
        }
    }

    private void SetText(string content)
    {
        Text = Check.NotNullOrWhiteSpace(
            content,
            nameof(content),
            MessageConsts.MaxContentLength,
            MessageConsts.MinContentLength);
    }

    private void SetMedia(Blob media)
    {
        Media = media;
    }
}
