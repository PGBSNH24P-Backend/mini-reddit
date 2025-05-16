using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace mini_reddit;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddAuthentication().AddBearerToken(IdentityConstants.BearerScheme);
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("create_post", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("create_post");
            });
        });

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                "Host=localhost;Port=5432;Database=minireddit;Username=postgres;Password=password"
            )
        );

        builder.Services.AddControllers();
        builder.Services.AddScoped<IPostService, DefaultPostService>();
        builder.Services.AddScoped<IPostRepository, EfPostRepository>();
        builder.Services.AddScoped<ICommentService, DefaultCommentService>();
        builder.Services.AddScoped<ICommentRepository, EfCommentRepository>();
        builder.Services.AddScoped<IUserService, DefaultUserService>();

        builder.Services.AddIdentityCore<UserEntity>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddApiEndpoints();

        var app = builder.Build();

        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.MapIdentityApi<UserEntity>();

        Task.WaitAll([CreateDefaultRoles(app)]);

        app.Run();
    }

    private static StaticRole[] ROLES = [
        new StaticRole {
            Name = "admin",
            ChildRole = "mod",
            Claims = ["delete_other_user"]
        },
        new StaticRole {
            Name = "mod",
            ChildRole = "user",
            Claims = ["delete_other_comment"]
        },
        new StaticRole {
            Name = "user",
            ChildRole = null,
            Claims = ["create_post", "delete_self_post"]
        },
    ];

    private static ICollection<string> GetRoleClaims(string roleName)
    {
        var claims = new List<string>();

        var role = ROLES.FirstOrDefault(role => role.Name.Equals(roleName));
        if (role == null)
        {
            return claims;
        }

        claims.AddRange(role.Claims);
        if (role.ChildRole != null)
        {
            claims.AddRange(GetRoleClaims(role.ChildRole));
        }

        return claims;
    }

    static async Task CreateDefaultRole(RoleManager<IdentityRole> roleManager, string roleName)
    {
        if (await roleManager.FindByNameAsync(roleName) == null)
        {
            var role = new IdentityRole(roleName);
            await roleManager.CreateAsync(role);

            var claims = GetRoleClaims(roleName);
            foreach (var claim in claims)
            {
                await roleManager.AddClaimAsync(role, new Claim(claim, ""));
            }
        }
    }

    static async Task CreateDefaultRoles(WebApplication app)
    {
        using var scope = app.Services.CreateAsyncScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        foreach (var role in ROLES)
        {
            await CreateDefaultRole(roleManager, role.Name);
        }
    }
}


public class StaticRole
{
    public string Name { get; init; }
    public string? ChildRole { get; init; }
    public string[] Claims { get; init; }
}