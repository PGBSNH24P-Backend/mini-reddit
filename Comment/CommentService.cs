public interface ICommentService
{
    public Task<CommentEntity> CreateCommentAsync(Guid postId, CreateCommentRequest request);
    public Task<CommentEntity> CreateCommentForCommentAsync(
        Guid commentId,
        CreateCommentRequest request
    );

    public Task<CommentEntity?> DeleteCommentAsync(Guid commentId);
}

public class DefaultCommentService : ICommentService
{
    private readonly ICommentRepository commentRepository;
    private readonly IPostRepository postRepository;

    public DefaultCommentService(
        ICommentRepository commentRepository,
        IPostRepository postRepository
    )
    {
        this.commentRepository = commentRepository;
        this.postRepository = postRepository;
    }

    public async Task<CommentEntity> CreateCommentAsync(Guid postId, CreateCommentRequest request)
    {
        if (string.IsNullOrEmpty(request.Content))
        {
            throw new ArgumentException("Content may not be null or empty");
        }

        var postEntity = await postRepository.GetByIdAsync(postId);
        if (postEntity == null)
        {
            throw new KeyNotFoundException("Post with id not found");
        }

        var commentEntity = new CommentEntity(request.Content, postEntity);
        postEntity.Comments.Add(commentEntity);
        await commentRepository.AddAsync(commentEntity);

        return commentEntity;
    }

    public async Task<CommentEntity> CreateCommentForCommentAsync(
        Guid parentCommentId,
        CreateCommentRequest request
    )
    {
        if (string.IsNullOrEmpty(request.Content))
        {
            throw new ArgumentException("Content may not be null or empty");
        }

        var parentCommentEntity = await commentRepository.GetByIdAsync(parentCommentId);
        if (parentCommentEntity == null)
        {
            throw new KeyNotFoundException("Comment with id not found");
        }

        var commentEntity = new CommentEntity(
            request.Content,
            parentCommentEntity.Post,
            parentCommentEntity
        );
        parentCommentEntity.Post.Comments.Add(commentEntity);
        await commentRepository.AddAsync(commentEntity);

        return commentEntity;
    }

    public async Task<CommentEntity?> DeleteCommentAsync(Guid commentId)
    {
        var commentEntity = await commentRepository.GetByIdAsync(commentId);
        if (commentEntity == null)
        {
            return null;
        }

        await commentRepository.DeleteAsync(commentEntity);
        return commentEntity;
    }
}
