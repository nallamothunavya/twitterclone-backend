namespace Twittertask.Models;

public record Comment
{
    public long Id { get; set; }

    public string Text { get; set; }
    public long UserId { get; set; }

    public long PostId { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

}