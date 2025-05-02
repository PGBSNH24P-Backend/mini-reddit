public interface IPostService
{
    public Task<PostEntity> CreatePostAsync(CreatePostRequest request);
    public Task<PageResult<PostEntity>> GetPageAsync(int page);
    public Task<PostEntity?> DeletePostAsync(Guid postId);
    public Task<PostEntity?> ReactPostAsync(Guid postId, ReactionType reactionType);
    public Task<PostEntity?> GetPostByIdAsync(Guid postId);
}

public class DefaultPostService : IPostService
{
    private readonly IPostRepository postRepository;

    public DefaultPostService(IPostRepository postRepository)
    {
        this.postRepository = postRepository;
    }

    public async Task<PostEntity> CreatePostAsync(CreatePostRequest request)
    {
        if (string.IsNullOrEmpty(request.Title))
        {
            throw new ArgumentException("Title may not be null or empty");
        }

        if (request.Title.Length < 3)
        {
            throw new ArgumentException("Title must be at least 3 characters");
        }

        if (string.IsNullOrEmpty(request.Content))
        {
            throw new ArgumentException("Content may not be null or empty");
        }

        var postEntity = new PostEntity(request.Title, request.Content);
        await postRepository.AddAsync(postEntity);

        return postEntity;
    }

    public async Task<PageResult<PostEntity>> GetPageAsync(int page)
    {
        int pageSize = 5;
        var posts = await postRepository.GetPageAsync(page, pageSize);
        var count = await postRepository.CountAsync();

        var hasPrevious = page > 0 && count >= pageSize;
        var hasNext = page * pageSize + pageSize < count;

        return new PageResult<PostEntity>
        {
            Page = posts,
            HasNext = hasNext,
            HasPrevious = hasPrevious,
        };
    }

    public async Task<PostEntity?> DeletePostAsync(Guid postId)
    {
        var postEntity = await postRepository.GetByIdAsync(postId);
        if (postEntity == null)
        {
            return null;
        }

        await postRepository.DeleteAsync(postEntity);
        return postEntity;
    }

    public async Task<PostEntity?> ReactPostAsync(Guid postId, ReactionType reactionType)
    {
        var postEntity = await postRepository.GetByIdAsync(postId);
        if (postEntity == null)
        {
            return null;
        }

        var reactionEntity = new ReactionEntity(reactionType, postEntity);
        postEntity.Reactions.Add(reactionEntity);

        await postRepository.AddReactionAsync(reactionEntity);

        return postEntity;
    }

    public async Task<PostEntity?> GetPostByIdAsync(Guid postId)
    {
        return await postRepository.GetByIdAsync(postId);
    }
}
