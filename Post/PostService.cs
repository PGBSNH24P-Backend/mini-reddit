public interface IPostService
{
    public Task<PostEntity> CreatePostAsync(string userId, CreatePostRequest request);
    public Task<PageResult<PostEntity>> GetPageAsync(int page);
    public Task<PostEntity?> DeletePostAsync(string userId, Guid postId);
    public Task ReactPostAsync(string userId, Guid postId, ReactionType reactionType);
    public Task<PostEntity?> GetPostByIdAsync(Guid postId);
}

public class DefaultPostService : IPostService
{
    private readonly IPostRepository postRepository;
    private readonly IUserService userService;

    public DefaultPostService(IPostRepository postRepository, IUserService userService)
    {
        this.postRepository = postRepository;
        this.userService = userService;
    }

    public async Task<PostEntity> CreatePostAsync(string userId, CreatePostRequest request)
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

        var user = await userService.GetUserByIdAsync(userId);
        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        var postEntity = new PostEntity(request.Title, request.Content, user);
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

    public async Task<PostEntity?> DeletePostAsync(string userId, Guid postId)
    {
        var postEntity = await postRepository.GetByIdAsync(postId);
        if (postEntity == null)
        {
            return null;
        }

        if (!postEntity.CreatedBy.Id.Equals(userId))
        {
            throw new UnauthorizedAccessException();
        }

        await postRepository.DeleteAsync(postEntity);
        return postEntity;
    }

    public async Task ReactPostAsync(string userId, Guid postId, ReactionType reactionType)
    {
        var existingReaction = await postRepository.GetReactionByPostAndUser(userId, postId);
        if (existingReaction != null)
        {
            existingReaction.Type = reactionType;
            await postRepository.SaveReaction(existingReaction);
            return;
        }

        var user = await userService.GetUserByIdAsync(userId);
        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        var post = await postRepository.GetByIdAsync(postId);
        if (post == null)
        {
            throw new KeyNotFoundException("Post not found");
        }

        var reactionEntity = new ReactionEntity(reactionType, post, user);
        post.Reactions.Add(reactionEntity);

        await postRepository.AddReactionAsync(reactionEntity);
    }

    public async Task<PostEntity?> GetPostByIdAsync(Guid postId)
    {
        return await postRepository.GetByIdAsync(postId);
    }
}
