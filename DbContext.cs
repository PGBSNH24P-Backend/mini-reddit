using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<PostEntity> Posts { get; set; }
    public DbSet<CommentEntity> Comments { get; set; }
    public DbSet<ReactionEntity> Reactions { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .Entity<CommentEntity>()
            .HasOne(model => model.ParentComment)
            .WithMany(model => model.SubComments)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
