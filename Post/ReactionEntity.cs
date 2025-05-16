public class ReactionEntity
{
    public Guid Id { get; set; }

    public PostEntity Post { get; set; }
    public UserEntity User { get; set; }

    public ReactionType Type { get; set; }

    public ReactionEntity(ReactionType type, PostEntity post, UserEntity user)
    {
        this.Id = Guid.NewGuid();
        this.Post = post;
        this.User = user;
        this.Type = type;
    }

    public ReactionEntity()
    {
        this.Post = null!;
        this.User = null!;
    }
}

public enum ReactionType
{
    Like,
    Dislike,
    Heart,
    Funny,
}
