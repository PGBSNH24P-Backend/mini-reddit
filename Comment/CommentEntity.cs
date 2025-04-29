public class CommentEntity
{
    public Guid Id { get; set; }

    public string Content { get; set; }

    public PostEntity Post { get; set; }

    public CommentEntity(string content, PostEntity post)
    {
        this.Id = Guid.NewGuid();
        this.Content = content;
        this.Post = post;
    }

    public CommentEntity()
    {
        this.Content = string.Empty;
        this.Post = null!;
    }
}