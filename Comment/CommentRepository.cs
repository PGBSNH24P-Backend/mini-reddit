using Microsoft.EntityFrameworkCore;

public interface ICommentRepository
{
    public Task AddAsync(CommentEntity entity);
    public Task<CommentEntity?> GetByIdAsync(Guid commentId);
    public Task DeleteAsync(CommentEntity entity);
}

public class EfCommentRepository : ICommentRepository
{
    private readonly AppDbContext context;

    public EfCommentRepository(AppDbContext context)
    {
        this.context = context;
    }

    public async Task AddAsync(CommentEntity entity)
    {
        await context.Comments.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(CommentEntity entity)
    {
        context.Comments.Remove(entity);
        await context.SaveChangesAsync();
    }

    public async Task<CommentEntity?> GetByIdAsync(Guid commentId)
    {
        return await context
            .Comments.Include(model => model.Post)
            .Include(model => model.SubComments)
            .FirstOrDefaultAsync(comment => comment.Id.Equals(commentId));
    }
}
