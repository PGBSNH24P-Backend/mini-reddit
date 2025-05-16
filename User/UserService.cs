using Microsoft.AspNetCore.Identity;

public interface IUserService
{
    public Task<UserEntity?> GetUserByIdAsync(string userId);
    public Task<UserEntity> CreateUserAsync(RegisterUserRequest request);
}

public class DefaultUserService : IUserService
{
    private readonly UserManager<UserEntity> userManager;

    public DefaultUserService(UserManager<UserEntity> userManager)
    {
        this.userManager = userManager;
    }

    public async Task<UserEntity> CreateUserAsync(RegisterUserRequest request)
    {
        var user = new UserEntity(request.UserName, request.Email);
        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            throw new IdentityException(result);
        }

        await userManager.AddToRoleAsync(user, "user");

        return user;
    }

    public async Task<UserEntity?> GetUserByIdAsync(string userId)
    {
        return await userManager.FindByIdAsync(userId);
    }
}