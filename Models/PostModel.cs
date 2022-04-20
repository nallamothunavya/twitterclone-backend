namespace Twittertask.Models;

public record Post
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string Title { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

}