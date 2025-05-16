public interface ICommentService
{
    public Task<CommentEntity> CreateCommentAsync(string userId, Guid postId, CreateCommentRequest request);
    public Task<CommentEntity> CreateCommentForCommentAsync(
        string userId,
        Guid commentId,
        CreateCommentRequest request
    );

    public Task<CommentEntity?> DeleteCommentAsync(string userId, Guid commentId);
}

public class DefaultCommentService : ICommentService
{
    private readonly ICommentRepository commentRepository;
    private readonly IPostRepository postRepository;
    private readonly IUserService userService;

    public DefaultCommentService(
        ICommentRepository commentRepository,
        IPostRepository postRepository,
        IUserService userService
    )
    {
        this.commentRepository = commentRepository;
        this.postRepository = postRepository;
        this.userService = userService;
    }

    public async Task<CommentEntity> CreateCommentAsync(string userId, Guid postId, CreateCommentRequest request)
    {
        if (string.IsNullOrEmpty(request.Content))
        {
            throw new ArgumentException("Content may not be null or empty");
        }

        var user = await userService.GetUserByIdAsync(userId);
        if (user == null)
        {
            throw new UnauthorizedAccessException();
        }

        var postEntity = await postRepository.GetByIdAsync(postId);
        if (postEntity == null)
        {
            throw new KeyNotFoundException("Post with id not found");
        }

        var commentEntity = new CommentEntity(request.Content, postEntity, user);
        postEntity.Comments.Add(commentEntity);
        await commentRepository.AddAsync(commentEntity);

        return commentEntity;
    }

    public async Task<CommentEntity> CreateCommentForCommentAsync(
        string userId,
        Guid parentCommentId,
        CreateCommentRequest request
    )
    {
        if (string.IsNullOrEmpty(request.Content))
        {
            throw new ArgumentException("Content may not be null or empty");
        }

        var user = await userService.GetUserByIdAsync(userId);
        if (user == null)
        {
            throw new UnauthorizedAccessException();
        }

        var parentCommentEntity = await commentRepository.GetByIdAsync(parentCommentId);
        if (parentCommentEntity == null)
        {
            throw new KeyNotFoundException("Comment with id not found");
        }

        var commentEntity = new CommentEntity(
            request.Content,
            parentCommentEntity.Post,
            user,
            parentCommentEntity
        );
        parentCommentEntity.Post.Comments.Add(commentEntity);
        await commentRepository.AddAsync(commentEntity);

        return commentEntity;
    }

    public async Task<CommentEntity?> DeleteCommentAsync(string userId, Guid commentId)
    {
        var commentEntity = await commentRepository.GetByIdAsync(commentId);
        if (commentEntity == null)
        {
            return null;
        }

        if (!commentEntity.CreatedBy.Id.Equals(userId))
        {
            throw new UnauthorizedAccessException();
        }

        await commentRepository.DeleteAsync(commentEntity);
        return commentEntity;
    }
}
