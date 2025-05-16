public class CommentEntity
{
    public Guid Id { get; set; }

    public string Content { get; set; }

    public PostEntity Post { get; set; }
    public CommentEntity? ParentComment { get; set; }
    public ICollection<CommentEntity> SubComments { get; set; }
    public UserEntity CreatedBy { get; set; }

    public CommentEntity(string content, PostEntity post, UserEntity user, CommentEntity? parentComment = null)
    {
        this.Id = Guid.NewGuid();
        this.Content = content;
        this.Post = post;
        this.CreatedBy = user;
        this.ParentComment = parentComment;
        this.SubComments = [];
    }

    public CommentEntity()
    {
        this.Content = string.Empty;
        this.Post = null!;
        this.CreatedBy = null!;
        this.ParentComment = null;
        this.SubComments = [];
    }
}
