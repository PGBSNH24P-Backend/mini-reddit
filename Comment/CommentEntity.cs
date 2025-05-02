public class CommentEntity
{
    public Guid Id { get; set; }

    public string Content { get; set; }

    public PostEntity Post { get; set; }
    public CommentEntity? ParentComment { get; set; }
    public ICollection<CommentEntity> SubComments { get; set; }

    public CommentEntity(string content, PostEntity post, CommentEntity? parentComment = null)
    {
        this.Id = Guid.NewGuid();
        this.Content = content;
        this.Post = post;
        this.ParentComment = parentComment;
        this.SubComments = [];
    }

    public CommentEntity()
    {
        this.Content = string.Empty;
        this.Post = null!;
        this.ParentComment = null;
        this.SubComments = [];
    }
}
