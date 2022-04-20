using System.Text.Json.Serialization;

namespace Twittertask.Models;

public record User
{

    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("full_name")]
    public string Fullname { get; set; }

    [JsonPropertyName("password")]
    public string Password { get; set; }

    [JsonPropertyName("email")]

    public string Email { get; set; }
}