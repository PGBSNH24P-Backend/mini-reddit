using Microsoft.EntityFrameworkCore;

public interface IPostRepository
{
    public Task AddAsync(PostEntity entity);
    public Task<IEnumerable<PostEntity>> GetPageAsync(int page, int pageSize);
    public Task<int> CountAsync();
    public Task<PostEntity?> GetByIdAsync(Guid postId);
    public Task DeleteAsync(PostEntity entity);
    public Task AddReactionAsync(ReactionEntity entity);
}

public class EfPostRepository : IPostRepository
{
    private readonly AppDbContext context;

    public EfPostRepository(AppDbContext context)
    {
        this.context = context;
    }

    public async Task AddAsync(PostEntity entity)
    {
        await context.Posts.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<PostEntity>> GetPageAsync(int page, int pageSize)
    {
        var posts = await context
            .Posts.Include(model => model.Reactions)
            .Include(model => model.Comments)
            .ThenInclude(comment => comment.SubComments)
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return posts;
    }

    public async Task<int> CountAsync()
    {
        return await context.Posts.CountAsync();
    }

    public async Task<PostEntity?> GetByIdAsync(Guid postId)
    {
        return await context
            .Posts.Include(model => model.Reactions)
            .Include(model => model.Comments)
            .ThenInclude(comment => comment.SubComments)
            .FirstOrDefaultAsync(post => post.Id.Equals(postId));
    }

    public async Task DeleteAsync(PostEntity entity)
    {
        context.Posts.Remove(entity);
        await context.SaveChangesAsync();
    }

    public async Task AddReactionAsync(ReactionEntity entity)
    {
        await context.Reactions.AddAsync(entity);
        await context.SaveChangesAsync();
    }
}
