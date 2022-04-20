using System.ComponentModel.DataAnnotations;

namespace Twittertask.DTOs;

public record CommentCreateDTO
{


    [Required]
    public long UserId { get; set; }

    [Required]
    public long PostId { get; set; }


    [Required]
    public string Text { get; set; }


    [Required]
    public DateTimeOffset CreatedAt { get; set; }



    [Required]
    public DateTimeOffset UpdatedAt { get; set; }



}

public record CommentUpdateDTO
{

    [Required]
    public long UserId { get; set; }

    [Required]
    public long PostId { get; set; }


    [Required]
    public string Text { get; set; }
}
