public class PostEntity
{
    public Guid Id { get; set; }

    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime CreationDate { get; set; }

    public ICollection<CommentEntity> Comments { get; set; }

    public ICollection<ReactionEntity> Reactions { get; set; }
    public UserEntity CreatedBy { get; set; }

    public PostEntity(string title, string content, UserEntity user)
    {
        this.Id = Guid.NewGuid();
        this.Title = title;
        this.Content = content;
        this.CreationDate = DateTime.UtcNow;
        this.CreatedBy = user;
        this.Comments = [];
        this.Reactions = [];
    }

    public PostEntity()
    {
        this.Title = string.Empty;
        this.Content = string.Empty;
        this.CreatedBy = null!;
        this.Comments = [];
        this.Reactions = [];
    }
}
