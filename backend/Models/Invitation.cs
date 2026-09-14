using System.Text.Json.Serialization;

namespace Hegerovi.Backend.Models;

public enum InvitationStatus
{
    Pending,
    Confirmed,
    Declined
}

public class Invitation
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string GuestId { get; set; } = string.Empty;
    public string Token { get; set; } = Guid.NewGuid().ToString("N");
    public InvitationStatus Status { get; set; } = InvitationStatus.Pending;
    public int PeopleCount { get; set; }
    public string? MenuPreference { get; set; }
    public string? Allergies { get; set; }
    public DateTimeOffset? RespondedAt { get; set; }
}
