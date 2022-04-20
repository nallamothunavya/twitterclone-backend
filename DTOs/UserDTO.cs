using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Twittertask.DTOs;

public record UserLoginDTO
{
    [Required]
    [JsonPropertyName("email")]
    [MinLength(3)]
    [MaxLength(255)]
    public string Email { get; set; }

    [Required]
    [JsonPropertyName("password")]
    [MaxLength(255)]
    public string Password { get; set; }
}

public record UserLoginResDTO
{
    [JsonPropertyName("token")]
    public string Token { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("id")]
    public long Id { get; set; }
}

public record UserCreateDto
{


    [JsonPropertyName("full_name")]
    [Required]
    [MaxLength(50)]
    public string Fullname { get; set; }


    [JsonPropertyName("password")]
    [Required]
    public string Password { get; set; }

    [JsonPropertyName("email")]
    [MaxLength(255)]
    public string Email { get; set; }

}

public record UserUpdateDto
{


    [JsonPropertyName("Fullname")]

    public string Fullname { get; set; }



}


