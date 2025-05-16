using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("user")]
public class UserController : ControllerBase
{

    private readonly IUserService userService;
    private readonly ILogger<UserController> logger;

    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        this.userService = userService;
        this.logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserRequest request)
    {
        try
        {
            await userService.CreateUserAsync(request);

            return NoContent();
        }
        catch (IdentityException exception)
        {
            return BadRequest(exception.Errors);
        }
        catch (Exception exception)
        {
            logger.LogError(
                            "Unexpected error when creating user: {} - {}",
                            exception.Message,
                            exception.StackTrace
                        );
            return StatusCode(500, new ApiError { Message = "Unexpected error" });
        }
    }
}

public class RegisterUserRequest
{
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}