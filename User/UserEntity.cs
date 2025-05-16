using Microsoft.AspNetCore.Identity;

public class UserEntity : IdentityUser
{
    public ICollection<PostEntity> Posts { get; set; }
    public ICollection<CommentEntity> Comments { get; set; }
    public ICollection<ReactionEntity> Reactions { get; set; }

    public UserEntity(string username, string email) : base(username)
    {
        this.Email = email;
        this.Posts = [];
        this.Comments = [];
        this.Reactions = [];
    }

    public UserEntity()
    {
        this.Posts = [];
        this.Comments = [];
        this.Reactions = [];
    }
}