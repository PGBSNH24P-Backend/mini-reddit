public class ReactionEntity
{
    public Guid Id { get; set; }

    public PostEntity Post { get; set; }

    public ReactionType Type { get; set; }

    public ReactionEntity(ReactionType type, PostEntity post)
    {
        this.Id = Guid.NewGuid();
        this.Post = post;
        this.Type = type;
    }

    public ReactionEntity()
    {
        this.Post = null!;
    }
}

public enum ReactionType
{
    Like,
    Dislike,
    Heart,
    Funny,
}
