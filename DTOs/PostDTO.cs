using System.ComponentModel.DataAnnotations;

namespace Twittertask.DTOs;

public record PostCreateDTO
{
    [Required]
    [MinLength(3)]
    [MaxLength(255)]
    public string Title { get; set; }

    [Required]
    public long UserId { get; set; }

    [Required]
    public DateTimeOffset CreatedAt { get; set; }



    [Required]
    public DateTimeOffset UpdatedAt { get; set; }


}

public record PostUpdateDTO
{
    [MinLength(3)]
    [MaxLength(255)]
    public string Title { get; set; } = null;

}
