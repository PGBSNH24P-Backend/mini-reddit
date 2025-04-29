using Microsoft.EntityFrameworkCore;

namespace mini_reddit;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(
            "Host=localhost;Port=5432;Database=minireddit;Username=postgres;Password=password"
        ));

        builder.Services.AddControllers();
        builder.Services.AddScoped<IPostService, DefaultPostService>();
        builder.Services.AddScoped<IPostRepository, EfPostRepository>();

        var app = builder.Build();

        app.MapControllers();

        app.Run();
    }
}

/*

PostController
CommentController

PostService
CommentService

PostRepository
CommentRepository

DbContext

*/