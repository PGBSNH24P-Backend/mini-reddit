public class CommentResponse
{

    public required Guid Id { get; set; }

    public required string Content { get; set; }

    public static CommentResponse FromEntity(CommentEntity entity)
    {
        return new CommentResponse
        {
            Id = entity.Id,
            Content = entity.Content,
        };
    }
}