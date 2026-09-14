using System.Text.Json.Serialization;

namespace Hegerovi.Backend.Models;

public class Guest
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string Group { get; set; } = string.Empty;
    public bool PlusOneAllowed { get; set; }
    public string? Note { get; set; }
}
