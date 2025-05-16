using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql.Replication;

[ApiController]
[Route("post")]
public class PostController : ControllerBase
{
    private readonly IPostService postService;
    private readonly ILogger<PostController> logger;

    public PostController(IPostService postService, ILogger<PostController> logger)
    {
        this.postService = postService;
        this.logger = logger;
    }

    [HttpPost("create")]
    [Authorize("create_post")]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostRequest request)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var postEntity = await postService.CreatePostAsync(userId, request);
            logger.LogInformation("Created post with id '{}'", postEntity.Id);
            return CreatedAtAction(nameof(CreatePost), PostResponse.FromEntity(postEntity));
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ApiError { Message = exception.Message });
        }
        catch (KeyNotFoundException)
        {
            return Unauthorized();
        }
        catch (Exception exception)
        {
            logger.LogError(
                "Unexpected error when creating post: {} - {}",
                exception.Message,
                exception.StackTrace
            );
            return StatusCode(500, new ApiError { Message = "Unexpected error" });
        }
    }

    [HttpGet("{postId}")]
    public async Task<IActionResult> GetPostById(Guid postId)
    {
        try
        {
            var postEntity = await postService.GetPostByIdAsync(postId);
            if (postEntity == null)
            {
                return NotFound(new ApiError { Message = "Post not found" });
            }

            return Ok(PostResponse.FromEntity(postEntity));
        }
        catch (Exception exception)
        {
            logger.LogError(
                "Unexpected error when fetching post: {} - {}",
                exception.Message,
                exception.StackTrace
            );
            return StatusCode(500, new ApiError { Message = "Unexpected error" });
        }
    }

    [HttpGet("page")]
    public async Task<IActionResult> GetPage([FromQuery] int page)
    {
        try
        {
            var result = await postService.GetPageAsync(page);
            return Ok(
                new PageResult<PostResponse>
                {
                    Page = result.Page.Select(post => PostResponse.FromEntity(post)),
                    HasNext = result.HasNext,
                    HasPrevious = result.HasPrevious,
                }
            );
        }
        catch (Exception exception)
        {
            logger.LogError(
                "Unexpected error when fetching post page: {} - {}",
                exception.Message,
                exception.StackTrace
            );
            return StatusCode(500, new ApiError { Message = "Unexpected error" });
        }
    }

    [HttpDelete("delete/{postId}")]
    [Authorize]
    public async Task<IActionResult> DeletePost(Guid postId)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var postEntity = await postService.DeletePostAsync(userId, postId);
            if (postEntity == null)
            {
                return NotFound(new ApiError { Message = "Post with id not found" });
            }

            logger.LogInformation("Deleted post with id '{}'", postEntity.Id);
            return Ok(postEntity.Id);
        }
        catch (UnauthorizedAccessException)
        {
            return NotFound();
        }
        catch (Exception exception)
        {
            logger.LogError(
                "Unexpected error when deleting post: {} - {}",
                exception.Message,
                exception.StackTrace
            );
            return StatusCode(500, new ApiError { Message = "Unexpected error" });
        }
    }

    [HttpPut("react/{postId}")]
    public async Task<IActionResult> ReactPost(Guid postId, [FromQuery] ReactionType reactionType)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            await postService.ReactPostAsync(userId, postId, reactionType);

            return NoContent();
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new ApiError { Message = exception.Message });

        }
        catch (Exception exception)
        {
            logger.LogError(
                "Unexpected error when reacting to post: {} - {}",
                exception.Message,
                exception.StackTrace
            );
            return StatusCode(500, new ApiError { Message = "Unexpected error" });
        }
    }
}

// DTO
public class CreatePostRequest
{
    public required string Title { get; set; }
    public required string Content { get; set; }
}

// DTO
public class PostResponse
{
    public required Guid Id { get; set; }

    public required string Title { get; set; }
    public required string Content { get; set; }
    public required DateTime CreationDate { get; set; }

    public required ICollection<CommentResponse> Comments { get; set; }

    public required Dictionary<string, int> Reactions { get; set; }
    public required string UserName { get; set; }

    public static PostResponse FromEntity(PostEntity entity)
    {
        var reactions = new Dictionary<string, int>();
        foreach (var reaction in entity.Reactions)
        {
            var currentAmount = reactions.GetValueOrDefault(reaction.Type.ToString(), 0);
            reactions[reaction.Type.ToString()] = currentAmount + 1;
        }

        return new PostResponse
        {
            Id = entity.Id,
            Title = entity.Title,
            Content = entity.Content,
            CreationDate = entity.CreationDate.ToLocalTime(),
            Comments = entity
                .Comments.Where(comment => comment.ParentComment == null)
                .Select(comment => CommentResponse.FromEntity(comment))
                .ToList(),
            Reactions = reactions,
            UserName = entity.CreatedBy?.UserName ?? "",
        };
    }
}

public class ReactionResponse
{
    public Guid Id { get; set; }

    public ReactionType Type { get; set; }

    public static ReactionResponse FromEntity(ReactionEntity entity)
    {
        return new ReactionResponse { Id = entity.Id, Type = entity.Type };
    }
}
