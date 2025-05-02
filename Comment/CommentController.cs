using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("comment")]
public class CommentController : ControllerBase
{
    private readonly ICommentService commentService;
    private readonly ILogger<CommentController> logger;

    public CommentController(ICommentService commentService, ILogger<CommentController> logger)
    {
        this.commentService = commentService;
        this.logger = logger;
    }

    [HttpPost("create/{postId}")]
    public async Task<IActionResult> CreateComment(
        Guid postId,
        [FromBody] CreateCommentRequest request
    )
    {
        try
        {
            var commentEntity = await commentService.CreateCommentAsync(postId, request);
            logger.LogInformation("Created comment with id '{}'", commentEntity.Id);

            return CreatedAtAction(
                nameof(CreateComment),
                CommentResponse.FromEntity(commentEntity)
            );
        }
        catch (ArgumentException exception)
        {
            return BadRequest(exception.Message);
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(exception.Message);
        }
        catch (Exception exception)
        {
            logger.LogError(
                "Unexpected error when creating comment: {} - {}",
                exception.Message,
                exception.StackTrace
            );
            return StatusCode(500, new ApiError { Message = "Unexpected error" });
        }
    }

    [HttpPost("create-for-comment/{commentId}")]
    public async Task<IActionResult> CreateCommentForComment(
        Guid commentId,
        [FromBody] CreateCommentRequest request
    )
    {
        try
        {
            var commentEntity = await commentService.CreateCommentForCommentAsync(
                commentId,
                request
            );
            logger.LogInformation("Created comment with id '{}'", commentEntity.Id);

            return CreatedAtAction(
                nameof(CreateCommentForComment),
                CommentResponse.FromEntity(commentEntity)
            );
        }
        catch (ArgumentException exception)
        {
            return BadRequest(exception.Message);
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(exception.Message);
        }
        catch (Exception exception)
        {
            logger.LogError(
                "Unexpected error when creating comment: {} - {}",
                exception.Message,
                exception.StackTrace
            );
            return StatusCode(500, new ApiError { Message = "Unexpected error" });
        }
    }

    [HttpDelete("delete/{commentId}")]
    public async Task<IActionResult> DeleteComment(Guid commentId)
    {
        try
        {
            var commentEntity = await commentService.DeleteCommentAsync(commentId);
            if (commentEntity == null)
            {
                return NotFound(new ApiError { Message = "Comment not found" });
            }

            return NoContent();
        }
        catch (Exception exception)
        {
            logger.LogError(
                "Unexpected error when deleting comment: {} - {}",
                exception.Message,
                exception.StackTrace
            );
            return StatusCode(500, new ApiError { Message = "Unexpected error" });
        }
    }
}

public class CreateCommentRequest
{
    public required string Content { get; set; }
}

public class CommentResponse
{
    public required Guid Id { get; set; }

    public required string Content { get; set; }
    public required IEnumerable<CommentResponse> SubComments { get; set; }

    public static CommentResponse FromEntity(CommentEntity entity)
    {
        return new CommentResponse
        {
            Id = entity.Id,
            Content = entity.Content,
            SubComments = entity.SubComments.Select(comment => CommentResponse.FromEntity(comment)),
        };
    }
}
