using System.Collections.ObjectModel;
using ProLinked.Domain.Extensions;
using ProLinked.Domain.Shared.Posts;
using ProLinked.Domain.Shared.Utils;

namespace ProLinked.Domain.Entities.Posts;

public class Post: Entity<Guid>
{
    public Guid CreatorId { get; init; }
    public string? Text { get; private set; }
    public PostVisibilityEnum PostVisibility { get; private set; }
    public ICollection<Comment> Comments { get; init; }
    public ICollection<Reaction> Reactions { get; init; }
    public ICollection<PostBlob> Media { get; init; }
    public DateTime CreationTime { get; init; }
    public DateTime LastModificationDate { get; private set; }

    private Post(Guid id): base(id){}

    public Post(
        Guid id,
        Guid creatorId,
        PostVisibilityEnum postVisibility,
        string? text = null,
        DateTime? creationTime = null)
        : base(id)
    {
        CreatorId = creatorId;
        SetVisibility(postVisibility);

        Comments = new Collection<Comment>();
        Reactions = new Collection<Reaction>();
        Media = new Collection<PostBlob>();
        CreationTime = creationTime ?? DateTime.Now;
        SetContent(text);
    }

    public Post UpdatePost(string? text = null, ICollection<PostBlob>? media = null)
    {
        SetContent(text, media);
        LastModificationDate = DateTime.Now;
        return this;
    }

    public Post SetContent(string? text = null, ICollection<PostBlob>? media = null)
    {
        if (!text.IsNullOrWhiteSpace())
        {
            SetText(text!);
        }

        if (media is not null)
        {
            SetMedia(media);
        }

        return this;
    }

    private void SetText(string text)
    {
        Text = Check.NotNullOrWhiteSpace(
            text,
            nameof(text),
            PostConsts.MaxContentLength,
            PostConsts.MinContentLength);
    }

    private void SetMedia(ICollection<PostBlob> mediaToAdd, bool overridePrevious = false)
    {
        if (overridePrevious)
        {
            Media.Clear();
        }

        foreach(PostBlob item in mediaToAdd)
        {
            Media.Add(item);
        }
    }


    public Post SetVisibility(PostVisibilityEnum postVisibility)
    {
        PostVisibility = postVisibility;
        return this;
    }

    public Post AddComment(Comment commentToAdd)
    {
        Comments.Add(commentToAdd);
        return this;
    }

    public Post AddReaction(Reaction reactionToAdd)
    {
        Reactions.Add(reactionToAdd);
        return this;
    }

    public Post RemoveReaction(Guid? reactionId)
    {
        var reactionToDelete = Reactions.FirstOrDefault(e => e.Id == reactionId);
        Reactions.Remove(reactionToDelete!);

        return this;
    }
}